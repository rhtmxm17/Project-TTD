using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCombatable : Combatable
{

    enum curState { WAITING, OTHERS }

    curState state = curState.WAITING;

    Vector3 originPos;
    //BasicSkillButton basicSkillButton;

    CharacterData characterData;

    protected override void Awake()
    {
        base.Awake();

        originPos = transform.position;
        waveClearEvent.AddListener(BackToOriginPos);

    }

    public void InitCharacterData(CharacterData characterData, BasicSkillButton basicSkillButton, SecondSkillButton secondSkillButton)
    {
        this.characterData = characterData;

        basicSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.SkillDataSO.SkillSprite;
        basicSkillButton.GetComponent<Button>().onClick.AddListener(() => {
            if (!basicSkillButton.Interactable) { Debug.Log("사용 불가");  return; }
            StartCoroutine(basicSkillButton.StartCoolDown(5));
        });

        secondSkillButton.SetSkillCost(characterData.StatusTable.SecondSkillCost);
        secondSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.SecondSkillDataSO.SkillSprite;
        secondSkillButton.GetComponent<Button>().onClick.AddListener(() => {
            if (!secondSkillButton.Interactable) { Debug.Log("사용 불가"); return; }
            if (characterData.StatusTable.SecondSkillCost < StageManager.Instance.PartyCost)
            {
                StageManager.Instance.UsePartyCost(characterData.StatusTable.SecondSkillCost);
                OnSkillCommanded(characterData.SecondSkillDataSO);
            }
        });
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