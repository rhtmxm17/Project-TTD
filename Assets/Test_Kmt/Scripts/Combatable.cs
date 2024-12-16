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
    [HideInInspector]
    public UnityEvent<GameObject> onDeadEvent = new UnityEvent<GameObject>();

    Trackable trackable;
    Coroutine curCombatCoroutine = null;

    Func<Transform, Transform> foundEnemyLogic = null;
    Func<Transform, Transform> foundNearEnemyLogic = null;
    Func<Transform, Transform> foundFarEnemyLogic = null;

    public enum SearchLogic { NEAR_FIRST, FAR_FIRST }

    protected virtual void Awake()
    {
        trackable = GetComponent<Trackable>();
        onDeadEvent.AddListener(GetComponentInParent<CombManager>().OnDead);

        foundNearEnemyLogic = againistObjList.GetNearestTrackable;
        foundFarEnemyLogic = againistObjList.GetFarestTrackable;

        InitSearchLogic();
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


    [ContextMenu("OnDead")]
    public void OnDead()
    {
        onDeadEvent?.Invoke(gameObject);
    }

    [ContextMenu("ChangeToNear")]
    public void ChangeSearchLogicToNear()
    {
        StopCoroutine(curCombatCoroutine);
        searchLogicType = SearchLogic.NEAR_FIRST;
        InitSearchLogic();
        curCombatCoroutine = StartCoroutine(CombatCO());
    }

    [ContextMenu("ChangeToFar")]
    public void ChangeSearchLogicToFar()
    {
        StopCoroutine(curCombatCoroutine);
        searchLogicType = SearchLogic.FAR_FIRST;
        InitSearchLogic();
        curCombatCoroutine = StartCoroutine(CombatCO());
    }

    public void ChangeSearchLoginInCombat(SearchLogic searchLogic)
    {
        StopCoroutine(curCombatCoroutine);
        searchLogicType = searchLogic;
        InitSearchLogic();
        curCombatCoroutine = StartCoroutine(CombatCO());
    }
    public void ChangeSearchLoginInCombat(Func<Transform, Transform> customSearchLogic)
    {
        StopCoroutine(curCombatCoroutine);
        foundEnemyLogic = customSearchLogic;
        curCombatCoroutine = StartCoroutine(CombatCO());
    }

    public void StartCombat(/*CombManager againistObjList;*/)
    {
        curCombatCoroutine = StartCoroutine(CombatCO());
    }


    //게임에서는 웨이브 자체를 전달. + 특성에 따라 추적 대상 함수등을 전달할수도..
    IEnumerator CombatCO(/*CombManager againistObjList;*/)
    {

        Transform ch = foundEnemyLogic.Invoke(transform);

        while (ch != null)
        {
            yield return StartCoroutine(trackable.TrackingCO(ch));//이동 코루틴

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

        waveClearEvent?.Invoke();

    }

    //테스트용 호출.
    //실제 전투 시작시, 캐릭터/몬스터측 combatCO이런거 호출.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCombat();
        }
    }

}
