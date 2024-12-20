using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

[RequireComponent(typeof(Trackable))]
public class Combatable : MonoBehaviour
{

    [SerializeField]
    CombManager againistObjList;
    [SerializeField]
    SearchLogic searchLogicType;

    [HideInInspector]
    public UnityEvent waveClearEvent = new UnityEvent();
    protected UnityEvent interruptedEvent = new UnityEvent();
    [HideInInspector]
    public UnityEvent<Combatable> onDeadEvent = new UnityEvent<Combatable>();

    protected Trackable trackable;
    protected Coroutine curActionCoroutine = null;
    protected Coroutine moveCoroutine = null;
    protected SkillButton SkillButton;

    Func<Transform, Transform> foundEnemyLogic = null;
    Func<Transform, Transform> foundNearEnemyLogic = null;
    Func<Transform, Transform> foundFarEnemyLogic = null;


    public enum SearchLogic { NEAR_FIRST, FAR_FIRST }

    protected virtual void Awake()
    {
        trackable = GetComponent<Trackable>();
        onDeadEvent.AddListener(GetComponentInParent<CombManager>().OnDead);

        AttackPoint = attackPoint.ToReadOnlyReactiveProperty();
        Hp = hp.ToReadOnlyReactiveProperty();
        MaxHp = maxHp.ToReadOnlyReactiveProperty();
    }

    public void Initialize(Animator animator, CombManager Group, CharacterData data)
    {
        UnitAnimator = animator;
        this.Group = Group;

    }

    #region TODO
    public Animator UnitAnimator { get; private set; }
    public CombManager Group { get; private set; }

    private ReactiveProperty<float> attackPoint = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> AttackPoint { get; private set; }

    private ReactiveProperty<float> hp = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> Hp;

    private ReactiveProperty<float> maxHp = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> MaxHp;

    public void Damaged(float damage)
    {
        Debug.Log($"피격데미지{damage}");
    }

    #endregion

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
        Debug.Log(gameObject.GetInstanceID());

        StopCurActionCoroutine();
        Debug.Log(gameObject.GetInstanceID());
        curActionCoroutine = StartCoroutine(TrackingCo());
        /*        Debug.Log(curActionCoroutine == null);
                Debug.Log(gameObject == null);*/
        Debug.Log("호출");
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
        Destroy(gameObject);
        onDeadEvent?.Invoke(this);
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
    public void ChangeSearchLoginInCombat(Func<Transform, Transform> customSearchLogic)
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
        Transform ch = foundEnemyLogic.Invoke(transform);
        float trackTime = 0.2f;
        float time = 0;
        int rangePow = 9;//사거리

        while (ch != null && againistObjList != null)
        {

            Vector3 moveDir = (ch.position - transform.position).normalized;

            while (ch != null && rangePow < Vector3.SqrMagnitude(ch.position - transform.position))
            {
                if (time > trackTime)
                {
                    time = 0;
                    moveDir = (ch.position - transform.position).normalized;
                }

                transform.Translate(10 * moveDir.normalized * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }


            if (ch == null)//이동중에 적이 쓰러진 경우.
            {
                ch = foundEnemyLogic.Invoke(transform);//새로운 대상 탐색
            }
            else
            {
                StopCurActionCoroutine();
                curActionCoroutine = StartCoroutine(CombatCO(ch));
                yield break;
            }


        }

        //전투가 끝난경우.
        EndCombat();

    }

    IEnumerator CombatCO(Transform ch)
    {
        while (ch != null && trackable.rangePow > Vector3.SqrMagnitude(ch.transform.position - transform.position))
        {
            //ch.transform.Rotate(Vector3.forward * 10);
            ch.GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV();
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