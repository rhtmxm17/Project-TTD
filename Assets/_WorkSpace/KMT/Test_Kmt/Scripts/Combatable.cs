using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Unity.Mathematics;
using UnityEngine.UI;
//using static Spine.Unity.Editor.SkeletonBaker.BoneWeightContainer;
using UnityEditor.U2D.Animation;

[RequireComponent(typeof(Trackable))]
public class Combatable : MonoBehaviour
{

    [SerializeField]
    CombManager againistObjList;
    [SerializeField]
    SearchLogic searchLogicType;

    [SerializeField]
    Slider hpSlider;

    [HideInInspector]
    public UnityEvent waveClearEvent = new UnityEvent();
    protected UnityEvent interruptedEvent = new UnityEvent();
    [HideInInspector]
    public UnityEvent<Combatable> onDeadEvent = new UnityEvent<Combatable>();
    public UnityEvent<float> onDamagedEvent = new UnityEvent<float>();

    protected Trackable trackable;
    protected Coroutine curActionCoroutine = null;
    protected Coroutine moveCoroutine = null;
    protected SkillButton SkillButton;

    Func<Transform, Combatable> foundEnemyLogic = null;
    Func<Transform, Combatable> foundNearEnemyLogic = null;
    Func<Transform, Combatable> foundFarEnemyLogic = null;

    int rangePow;
    Skill baseAttack;

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

        trackable = GetComponent<Trackable>();
        onDeadEvent.AddListener(GetComponentInParent<CombManager>().OnDead);

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

        // 외형 생성
        GameObject model = Instantiate(data.ModelPrefab, this.transform);
        model.name = "Model";
        if (false == model.TryGetComponent(out Animator animator))
        {
            animator = model.GetComponentInChildren<Animator>();
        }
        UnitAnimator = animator;

        // 그룹 지정
        this.Group = Group;

        IsAlive = true;

        // 레벨이 지정되지 않았을 경우 유저 데이터의 레벨 사용
        if (level < 0)
            level = data.Level.Value;

        CharacterData.Status table  = data.StatusTable;

        attackPoint.Value = table.attackPointBase
                          + table.attackPointGrowth * level;

        maxHp.Value = table.healthPointBase
                    + table.healthPointGrouth * level;

        hp.Value = MaxHp.Value;

        defense.Value = table.defensePointBase
                      + table.defensePointGrouth * level;

        defConst = table.defenseCon;
        
        rangePow = (int)(table.Range * table.Range);//사거리

        baseAttack = data.BasicSkillDataSO;

        hp.Subscribe(x => {

            hpSlider.value = x / MaxHp.Value;

        });
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

    public void Damaged(float damage, float igDefRate)
    {

        if (!IsAlive)
        {
            Debug.Log("이미 죽은 대상.");
            return;
        }

        damage = DamageCalculator.Calc(damage, igDefRate, defense.Value, defConst);

        //Debug.Log($"피격데미지{damage}");

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

    protected void StopCurActionCoroutine()//중지시키는거 더 고려
    {

        if (curActionCoroutine != null)
            StopCoroutine(curActionCoroutine);

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

    }

    #region 스킬_추가 
    public virtual void OnSkillCommanded(Skill skillData)
    {
        
        StopCurActionCoroutine();

        curActionCoroutine = StartCoroutine(skillData.SkillRoutine(this, OnSkillCompleted));
    }

    private void OnSkillCompleted()
    {

        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(TrackingCo());
        /*        Debug.Log(curActionCoroutine == null);
                Debug.Log(gameObject == null);*/
    }

    //private IEnumerator SkillRoutine(Skill skillData)
    //{   
    //    //TODO : 실제 스킬 발동시키기.
    //    Debug.Log($"{name} 캐릭터 : 스킬 사용 개시");

    //    yield return new WaitForSeconds(1.5f);


    //    Debug.Log($"{name} 캐릭터 : 스킬 종료, 자동 공격 시작");


    //    //스킬 사용 종료 후 자동 공격 다시 시작
    //    StopCurActionCoroutine();
    //    curActionCoroutine = StartCoroutine(TrackingCo());
    //}  

    #endregion

    [ContextMenu("OnDead")]
    public void OnDead()
    {
        //TODO : 애니메이션 코루틴같은거 추가
        Destroy(gameObject);
        onDeadEvent?.Invoke(this);
        StopCurActionCoroutine();
    }

    [ContextMenu("ChangeToNear")]
    public void ChangeSearchLogicToNear()
    {
        StopCurActionCoroutine();
        searchLogicType = SearchLogic.NEAR_FIRST;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    [ContextMenu("ChangeToFar")]
    public void ChangeSearchLogicToFar()
    {
        StopCurActionCoroutine();
        searchLogicType = SearchLogic.FAR_FIRST;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    public void ChangeSearchLoginInCombat(SearchLogic searchLogic)
    {
        StopCurActionCoroutine();
        searchLogicType = searchLogic;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(TrackingCo());
    }
    public void ChangeSearchLoginInCombat(Func<Transform, Combatable> customSearchLogic)
    {
        StopCurActionCoroutine();
        foundEnemyLogic = customSearchLogic;
        curActionCoroutine = StartCoroutine(TrackingCo());
    }

    public virtual void StartCombat(CombManager againstL)
    {
        againistObjList = againstL;

        foundNearEnemyLogic = againistObjList.GetNearestTrackable;
        foundFarEnemyLogic = againistObjList.GetFarestTrackable;

        InitSearchLogic();

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

        Combatable target = foundEnemyLogic.Invoke(transform);
        float trackTime = 0.2f;
        float time = 0;

        while (target != null && target.IsAlive && againistObjList != null)
        {

            Vector3 moveDir = (target.transform.position - transform.position).normalized;

            while (target != null && rangePow < Vector3.SqrMagnitude(target.transform.position - transform.position))
            {
                if (time > trackTime)
                {
                    time = 0;
                    moveDir = (target.transform.position - transform.position).normalized;
                }

                //TODO : 이동속도 상수 제거
                transform.Translate(10 * moveDir.normalized * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }


            if (!target.IsAlive)//이동중에 적이 쓰러진 경우.
            {
                target = foundEnemyLogic.Invoke(transform);//새로운 대상 탐색
            }
            else
            {
                StopCurActionCoroutine();
                curActionCoroutine = StartCoroutine(CombatCO(target));
                yield break;
            }

        }

        //전투가 끝난경우.
        EndCombat();

    }

    IEnumerator CombatCO(Combatable target)
    {
        yield return null;

        while (target.IsAlive && rangePow > Vector3.SqrMagnitude(target.transform.position - transform.position))
        {
            StartCoroutine(baseAttack.SkillRoutine(this, null));
            yield return new WaitForSeconds(1);
        }

        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(TrackingCo());
        yield break;
    }

    /*
        //게임에서는 웨이브 자체를 전달. + 특성에 따라 추적 대상 함수등을 전달할수도..
        IEnumerator CombatCO()
        {
            yield return null;
            Transform ch = foundEnemyLogic.Invoke(transform);

            //추적 대상이 있고, 웨이브가 진행중인 경우.
            while (ch != null && againistObjList != null)
            {
                moveCoroutine = StartCoroutine(trackable.TrackingCO(ch));
                yield return moveCoroutine;//이동 코루틴

                if (ch == null)//이동중에 적이 쓰러진 경우.
                {
                    ch = foundEnemyLogic.Invoke(transform);//새로운 대상 탐색
                    continue;
                }

                while (ch != null && trackable.rangePow > Vector3.SqrMagnitude(ch.transform.position - transform.position))
                {
                    //ch.transform.Rotate(Vector3.forward * 10);
                    ch.GetComponent<SpriteRenderer>().color = UnityEngine. Random.ColorHSV();
                    yield return new WaitForSeconds(1);
                }

                ch = foundEnemyLogic.Invoke(transform);//적이 사라진 뒤 다음 타깃 탐색

            }

            //전투가 끝난경우.
            EndCombat();

        }
    */

}