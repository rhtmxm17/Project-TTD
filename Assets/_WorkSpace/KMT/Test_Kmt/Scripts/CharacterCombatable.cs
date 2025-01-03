
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CharacterCombatable : Combatable
{

    enum curState { WAITING, OTHERS }

    curState state = curState.WAITING;

    Vector3 originPos;

    protected override void Awake()
    {
        base.Awake();

        originPos = transform.position;
        waveClearEvent.AddListener(BackToOriginPos);
    }

    public void InitCharacterData(BasicSkillButton basicSkillButton, SecondSkillButton secondSkillButton)
    {
        if (characterData == null)
        {
            Debug.LogError($"캐릭터 데이터 등록이 선행되어야 함");
        }

        basicSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.NormalSkillIcon;
        basicSkillButton.GetComponent<Button>().onClick.AddListener(() => {
            if (!basicSkillButton.Interactable || !IsAlive) { Debug.Log("사용 불가");  return; }
            OnSkillCommanded(characterData.SkillDataSO);
            basicSkillButton.StartCoolDown(characterData.StatusTable.BasicSkillCooldown);//쿨타임을 매개변수로 전달.
        });

        secondSkillButton.SetSkillCost(characterData.StatusTable.SecondSkillCost);
        secondSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.SpecialSkillIcon;
        secondSkillButton.GetComponent<Button>().onClick.AddListener(() => {
            if (!secondSkillButton.Interactable || !IsAlive) { Debug.Log("사용 불가"); return; }
            if (characterData.StatusTable.SecondSkillCost < StageManager.Instance.PartyCost)
            {
                StageManager.Instance.UsePartyCost(characterData.StatusTable.SecondSkillCost);
                OnSkillCommanded(characterData.SecondSkillDataSO);
            }
        });

        onDeadEvent.AddListener(basicSkillButton.OffSkillButton);
        onDeadEvent.AddListener(secondSkillButton.OffSkillButton);
    }

    public override void StartCombat(CombManager againstL)
    {
        state = curState.OTHERS;
        base.StartCombat(againstL);
    }

    public bool IsWaiting()
    {
        return state == curState.WAITING;
    }

    void BackToOriginPos()
    {
        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(BackToPosCO());
    }


    IEnumerator BackToPosCO()
    {
        yield return null;

        agent.stoppingDistance = 0.05f;
        agent.destination = originPos;

        while (agent.pathPending || 0.1f < agent.remainingDistance)
        {
            yield return null;
        }

        transform.position = originPos;

        agent.stoppingDistance = range;//TODO : 개체별 크기가 다른 경우, 해당 로직에 추가 수정.

        state = curState.WAITING;
    }

}