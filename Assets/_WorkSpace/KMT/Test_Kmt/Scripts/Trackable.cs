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

    //TweenerCore<Vector3, Vector3, VectorOptions> dtween = null;

    public IEnumerator TrackingCO(Transform dest)
    {
        float trackTime = 0.2f;
        float time = 0;

        Vector3 moveDir = (dest.position - transform.position).normalized;

        while (dest != null && rangePow < Vector3.SqrMagnitude(dest.position - transform.position))
        {
            if (time > trackTime)
            {
                time = 0;
                moveDir = (dest.position - transform.position).normalized;
            }

            transform.Translate(10 * moveDir.normalized * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

    }

}
