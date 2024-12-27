
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CharacterCombatable : Combatable
{

    enum curState { WAITING, OTHERS }

    curState state = curState.WAITING;

    Vector3 originPos;
    //BasicSkillButton basicSkillButton;

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

        basicSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.SkillDataSO.SkillSprite;
        basicSkillButton.GetComponent<Button>().onClick.AddListener(() => {
            if (!basicSkillButton.Interactable || !IsAlive) { Debug.Log("사용 불가");  return; }
            OnSkillCommanded(characterData.SkillDataSO);
            basicSkillButton.StartCoolDown(characterData.StatusTable.BasicSkillCooldown);//쿨타임을 매개변수로 전달하기.
        });

        secondSkillButton.SetSkillCost(characterData.StatusTable.SecondSkillCost);
        secondSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.SecondSkillDataSO.SkillSprite;
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

/*    public override void OnSkillCommanded(Skill skillData)//필요할지는 모름
    {
        
        base.OnSkillCommanded(skillData);
    }*/

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

        float trackTime = 0.2f;
        float time = 0;

        Vector3 moveDir = (originPos - transform.position).normalized;

        while (0.1f < Vector3.SqrMagnitude(originPos - transform.position))
        {
            if (time > trackTime)
            {
                time = 0;
                moveDir = (originPos - transform.position).normalized;
            }

            transform.Translate(10 * moveDir.normalized * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = originPos;
        state = curState.WAITING;
    }

}