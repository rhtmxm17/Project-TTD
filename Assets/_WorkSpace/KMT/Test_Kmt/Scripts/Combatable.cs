using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine.AI;
using Spine;

[RequireComponent(typeof(NavMeshAgent))]
public class Combatable : MonoBehaviour
{

    [SerializeField]
    CombManager againistObjList;

    [SerializeField]
    protected Slider hpSlider;

    [HideInInspector]
    public UnityEvent waveClearEvent = new UnityEvent();
    protected UnityEvent interruptedEvent = new UnityEvent();
    [HideInInspector]
    public UnityEvent<Combatable> onDeadEvent = new UnityEvent<Combatable>();
    public UnityEvent<float> onDamagedEvent = new UnityEvent<float>();

    protected Coroutine curActionCoroutine = null;

    protected float range;
    Skill baseAttack;

    protected NavMeshAgent agent;
    protected CharacterModel characterModel;

    [Header("TestParams")]
    [SerializeField]
    public float igDefenseRate;
    //무적 디버그를 위한 public
    [SerializeField]
    public float defConst;

    public bool IsAlive { get; private set; }

    public enum SearchLogic { NEAR_FIRST, FAR_FIRST }

    public CharacterData characterData { get; private set; }

    protected virtual void Awake()
    {
        onDeadEvent.AddListener(GetComponentInParent<CombManager>().OnDead);
        agent = GetComponent<NavMeshAgent>();

        AttackPoint = attackPoint.ToReadOnlyReactiveProperty();
        Hp = hp.ToReadOnlyReactiveProperty();
        MaxHp = maxHp.ToReadOnlyReactiveProperty();
        Defense = defense.ToReadOnlyReactiveProperty();
    }

    /// <summary>
    /// 전투 씬에서 캐릭터를 초기화
    /// </summary>
    /// <param name="Group">캐릭터가 속한 그룹(플레이어측 혹은 적 웨이브)</param>
    /// <param name="data">캐릭터 데이터</param>
    public void Initialize(CombManager Group, CharacterData data) => InitializeWithLevel(Group, data, data.Level.Value);

    /// <summary>
    /// 전투 씬에서 레벨을 지정해서 캐릭터를 초기화
    /// </summary>
    /// <param name="Group">캐릭터가 속한 그룹(플레이어측 혹은 적 웨이브)</param>
    /// <param name="data">캐릭터 데이터</param>
    /// <param name="level">레벨 지정</param>
    public void InitializeWithLevel(CombManager Group, CharacterData data, int level)
    {
        if (characterData != null)
        {
            Debug.LogWarning("캐릭터 초기화 함수가 두 번 실행됨");
        }
        characterData = data;

        gameObject.name = data.Name;

        SetCharacterModel(data.ModelPrefab);

        // 그룹 지정
        this.Group = Group;

        IsAlive = true;

        // 레벨이 지정되지 않았을 경우 유저 데이터의 레벨 사용
        if (level < 0)
            level = data.Level.Value;

        CharacterData.Status table = data.StatusTable;

        attackPoint.Value = table.attackPointBase
                          + table.attackPointGrowth * level;

        maxHp.Value = table.healthPointBase
                    + table.healthPointGrouth * level;

        hp.Value = MaxHp.Value;

        defense.Value = table.defensePointBase
                      + table.defensePointGrouth * level;

        defConst = table.defenseCon;

        agent.stoppingDistance = table.Range;
        range = table.Range;//사거리

        //TODO : 이동속도가 다른경우 파라미터 추가하기.

        baseAttack = data.BasicSkillDataSO;

        hp.Subscribe(x =>
        {

            hpSlider.value = x / MaxHp.Value;

        });
    }

    /// <summary>
    /// 모델을 세팅하는 함수
    /// </summary>
    /// <param name="modelData">세팅할 모델 프리팹</param>
    protected virtual void SetCharacterModel(CharacterModel modelData)
    {
        if(characterModel != null)
            Destroy(characterModel.gameObject);

        // 외형 생성
        characterModel = Instantiate(modelData, transform);
        characterModel.transform.rotation = Quaternion.Euler(90, 0, 0);
        characterModel.name = "Model";
        if (false == characterModel.TryGetComponent(out Animator animator))
        {
            animator = characterModel.GetComponentInChildren<Animator>();
        }
        UnitAnimator = animator;
    }

    public Animator UnitAnimator { get; private set; }
    public CombManager Group { get; private set; }

    protected ReactiveProperty<float> attackPoint = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> AttackPoint { get; private set; }

    protected ReactiveProperty<float> hp = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> Hp;

    protected ReactiveProperty<float> maxHp = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> MaxHp;

    protected ReactiveProperty<float> defense = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> Defense;

    public void Healed(float amount)
    {
        if (!IsAlive)
        {
            Debug.Log("이미 죽은 대상.");
            return;
        }

        float afterHp = MathF.Min(MaxHp.Value, hp.Value + amount);
        hp.Value = afterHp;

    }

    public void StatusBuffed(StatusBuffType type, float value)
    {
        switch (type)
        {
            case StatusBuffType.ATK_PERCENTAGE:
                attackPoint.Value *= (1f + value * 0.01f);
                break;
            case StatusBuffType.DEF_PERCENTAGE:
                defense.Value *= (1f + value * 0.01f);
                break;
            default:
                Debug.LogWarning("잘못되었거나 정의되지 않은 버프 타입");
                break;
        }
    }

    public void Damaged(float damage, float igDefRate)
    {
        if (!IsAlive)
        {
            Debug.Log("이미 죽은 대상.");
            return;
        }

        damage = DamageCalculator.Calc(damage, igDefRate, defense.Value, defConst);

        //View의 setvalue등을 연결하기.
        hp.Value -= damage;
        onDamagedEvent?.Invoke(damage);

        if (hp.Value < 0)
        {
            IsAlive = false;
            OnDead();
            Debug.Log("쓰러짐.");
        }


    }




    protected void StopCurActionCoroutine()
    {
        if (curActionCoroutine != null)
            StopCoroutine(curActionCoroutine);
    }

    #region 스킬_추가 
    /// <summary>
    /// 스킬의 타겟팅 로직을 실행해본 뒤, 타겟이 있다면 스킬을 실행.
    /// </summary>
    /// <param name="skillData">실행할 스킬 데이터</param>
    /// <returns>타겟 대상이 있는경우 스킬을 실행하고 true 반환, 없을경우 단순 false 반환.</returns>
    public bool OnSkillCommanded(Skill skillData)
    {
        Combatable skillTarget = skillData.TargetingLogic.GetTarget(this);
        if (skillTarget == null)
            return false;

        StopCurActionCoroutine();
        Look(skillTarget.transform);
        curActionCoroutine = StartCoroutine(skillData.SkillRoutine(this, skillTarget, OnSkillCompleted));
        agent.ResetPath();
        return true;
    }

    private void OnSkillCompleted()
    {
        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    #endregion

    [ContextMenu("OnDead")]
    public void OnDead()
    {
        //TODO : 애니메이션 코루틴같은거 추가
        Destroy(gameObject);
        onDeadEvent?.Invoke(this);
        StopCurActionCoroutine();
    }

    public virtual void StartCombat(CombManager againstL)
    {
        againistObjList = againstL;

        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    public void EndCombat()
    {
        againistObjList = null;
        StopCurActionCoroutine();
        waveClearEvent?.Invoke();
    }


    IEnumerator TrackingCo()
    {
        yield return null;

        Combatable target = baseAttack.TargetingLogic.GetTarget(this);

        float trackTime = 0.2f;
        float time = 0;

        while (target != null && target.IsAlive && againistObjList != null)
        {
            agent.stoppingDistance = range;//TODO : 개체별 크기가 다른 경우, 해당 로직에 추가 수정.
            agent.destination = target.transform.position;
            Look(target.transform);
            yield return new WaitWhile(() => agent.pathPending);

            while (target != null && agent.remainingDistance > agent.stoppingDistance)
            {
                if (time > trackTime)
                {
                    time = 0;

                    target = baseAttack.TargetingLogic.GetTarget(this);
                    if (target != null && target.IsAlive && againistObjList != null)
                    {
                        agent.destination = target.transform.position;
                        Look(target.transform);
                        yield return new WaitWhile(() => agent.pathPending);
                    }

                }

                time += Time.deltaTime;
                yield return null;
            }

            if (target == null)//타깃 null 체크
            {
                target = baseAttack.TargetingLogic.GetTarget(this);//새로운 대상 탐색
            }
            else if (!target.IsAlive)//이동중에 적이 쓰러진 경우.
            {
                target = baseAttack.TargetingLogic.GetTarget(this);//새로운 대상 탐색
            }
            else//적이 살아있고 사거리에 도착했을 경우.
            {
                StopCurActionCoroutine();
                curActionCoroutine = StartCoroutine(CombatCO(target));
                yield break;
            }

        }

        //전투가 끝난경우.
        EndCombat();

    }

    protected void Look(Transform target)
    {
        //본인이 타깃인 경우 보는 방향을 바꾸지 않음
        if (target == null || target == transform)
            return;

        Look(target.position);
    } 
    protected void Look(Vector3 target)
    {
        if (Vector3.Dot(target - transform.position, Vector3.right) > -0.1f)
        {
            characterModel.transform.localRotation = Quaternion.Euler(-90, -90, -90);
        }
        else
        {
            characterModel.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }

    IEnumerator CombatCO(Combatable target)
    {
        yield return null;

        while (target != null && target.IsAlive && range > Vector3.Distance(target.transform.position, transform.position))
        {
            Look(target.transform);
            StartCoroutine(baseAttack.SkillRoutine(this, target, null));
            yield return new WaitForSeconds(1);
        }

        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }


    // Legacy Targeting Logic
    #region 이전 타겟팅 코드 영역 [ 현재 사용되지 않음 ]

    SearchLogic searchLogicType;

    Func<Transform, Combatable> foundEnemyLogic = null;
    Func<Transform, Combatable> foundNearEnemyLogic = null;
    Func<Transform, Combatable> foundFarEnemyLogic = null;


    /// <summary>
    /// targeting타입 SO를 오버라이드하여 Skill에 지정하여 사용하세요.
    /// </summary>
    [Obsolete]
    void InitSearchLogic()
    {
        switch (searchLogicType)
        {
            case SearchLogic.NEAR_FIRST:
                foundEnemyLogic = foundNearEnemyLogic;
                break;
            case SearchLogic.FAR_FIRST:
                foundEnemyLogic = foundFarEnemyLogic;
                break;
            default:
                Debug.LogError("정의되지 않은 초기 로직.");
                break;
        }
    }

    [Obsolete]
    public void ChangeSearchLogicToNear()
    {
        StopCurActionCoroutine();
        searchLogicType = SearchLogic.NEAR_FIRST;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    [Obsolete]
    public void ChangeSearchLogicToFar()
    {
        StopCurActionCoroutine();
        searchLogicType = SearchLogic.FAR_FIRST;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    [Obsolete]
    public void ChangeSearchLoginInCombat(SearchLogic searchLogic)
    {
        StopCurActionCoroutine();
        searchLogicType = searchLogic;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    [Obsolete]
    public void ChangeSearchLoginInCombat(Func<Transform, Combatable> customSearchLogic)
    {
        StopCurActionCoroutine();
        foundEnemyLogic = customSearchLogic;
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    #endregion

}