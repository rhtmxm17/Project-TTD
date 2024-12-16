using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatable : Combatable
{
    Vector3 originPos;

    protected override void Awake()
    {
        base.Awake();

        originPos = transform.position;
        waveClearEvent.AddListener(BackToOriginPos);

    }

    public void BackToOriginPos()
    {

        transform
            .DOMove(originPos, 10)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear);

    }//캐릭터에만 따로 필요

}
