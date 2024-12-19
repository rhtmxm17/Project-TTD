using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatable : Combatable
{
    Vector3 originPos;
    TweenerCore<Vector3, Vector3, VectorOptions> moveTween = null;

    protected override void Awake()
    {
        base.Awake();

        originPos = transform.position;
        waveClearEvent.AddListener(BackToOriginPos);

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
    }

/*    public void BackToOriginPos2()
    {

        moveTween = transform
            .DOMove(originPos, 10)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear);
        moveTween.onComplete += () => { moveTween.Kill(); moveTween = null; };
            

    }//캐릭터에만 따로 필요

    void StopMove()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
            moveTween = null;
        }
    }*/

}
