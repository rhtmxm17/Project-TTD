using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatable : Combatable
{

    enum curState { WAITING, OTHERS }

    curState state = curState.WAITING;

    Vector3 originPos;
    SkillButton skillButton;

    protected override void Awake()
    {
        base.Awake();

        originPos = transform.position;
        waveClearEvent.AddListener(BackToOriginPos);

    }

    public void SetSkillButton(SkillButton skillButton)
    {
        this.skillButton = skillButton;
    }

    public override void OnSkillCommanded(Skill skillData)//필요할지는 모름
    {
        if (!skillButton.Interactable)
        {
            Debug.Log("쿨다운중");
            return;
        }

        StartCoroutine(skillButton.StartCoolDown(5));//TODO : 쿨타임 지정
        
        base.OnSkillCommanded(skillData);
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