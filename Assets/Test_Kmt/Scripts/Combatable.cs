using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent<GameObject> onDeadEvent = new UnityEvent<GameObject>();

    protected Trackable trackable;
    protected Coroutine curActionCoroutine = null;
    protected Coroutine moveCoroutine = null;
    
    Func<Transform, Transform> foundEnemyLogic = null;
    Func<Transform, Transform> foundNearEnemyLogic = null;
    Func<Transform, Transform> foundFarEnemyLogic = null;

    public enum SearchLogic { NEAR_FIRST, FAR_FIRST }

    protected virtual void Awake()
    {
        trackable = GetComponent<Trackable>();
        onDeadEvent.AddListener(GetComponentInParent<CombManager>().OnDead);
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

        if(curActionCoroutine != null)
            StopCoroutine(curActionCoroutine);

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

    }

    Coroutine skillRoutine;

    #region 스킬_추가 
    public void OnSkillCommanded(Skill skillData)
    {
        StopCurActionCoroutine();
        
        curActionCoroutine = StartCoroutine(SkillRoutine(skillData));
    }

    private IEnumerator SkillRoutine(Skill skillData)
    {   
        //TODO : 실제 스킬 발동시키기.
        Debug.Log($"{name} 캐릭터 : 스킬 사용 개시");

        yield return new WaitForSeconds(1.5f);
        
        Debug.Log($"{name} 캐릭터 : 스킬 종료, 자동 공격 시작");


        //스킬 사용 종료 후 자동 공격 다시 시작
        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(CombatCO());
    }  
    
    #endregion
     
    [ContextMenu("OnDead")]
    public void OnDead()
    {
        Destroy(gameObject);
        onDeadEvent?.Invoke(gameObject);
    }

    [ContextMenu("ChangeToNear")]
    public void ChangeSearchLogicToNear()
    {
        StopCurActionCoroutine();
        searchLogicType = SearchLogic.NEAR_FIRST;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(CombatCO());
    }

    [ContextMenu("ChangeToFar")]
    public void ChangeSearchLogicToFar()
    {
        StopCurActionCoroutine();
        searchLogicType = SearchLogic.FAR_FIRST;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(CombatCO());
    }

    public void ChangeSearchLoginInCombat(SearchLogic searchLogic)
    {
        StopCurActionCoroutine();
        searchLogicType = searchLogic;
        InitSearchLogic();
        curActionCoroutine = StartCoroutine(CombatCO());
    }
    public void ChangeSearchLoginInCombat(Func<Transform, Transform> customSearchLogic)
    {
        StopCurActionCoroutine();
        foundEnemyLogic = customSearchLogic;
        curActionCoroutine = StartCoroutine(CombatCO());
    }

    public void StartCombat(CombManager againstL)
    {
        againistObjList = againstL;

        foundNearEnemyLogic = againistObjList.GetNearestTrackable;
        foundFarEnemyLogic = againistObjList.GetFarestTrackable;

        InitSearchLogic();

        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(CombatCO());
    }

    public void EndCombat()
    {
        againistObjList = null;
        StopCurActionCoroutine();
        waveClearEvent?.Invoke();

    }



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
                ch.transform.Rotate(Vector3.forward * 10);
                yield return new WaitForSeconds(1);
            }

            ch = foundEnemyLogic.Invoke(transform);//적이 사라진 뒤 다음 타깃 탐색

        }

        //전투가 끝난경우.
        EndCombat();

    }


}
