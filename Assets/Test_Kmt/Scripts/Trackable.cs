using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trackable : MonoBehaviour
{

    [SerializeField]        //TODO : 
    public float rangePow = 9;//3 * 3 => 캐릭터 파라미터? 다른 컴포넌트로 이동


    public IEnumerator TrackingCO(Transform dest)
    {
        TweenerCore<Vector3, Vector3, VectorOptions> dtween = null;

        while (dest != null && rangePow < Vector3.SqrMagnitude(dest.position - transform.position))
        {
            dtween = transform
            .DOMove(dest.position, 10)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.02f);
            dtween.Kill();
        }

    }

}
