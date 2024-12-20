using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HYJ_UnscaledTimeTest : MonoBehaviour
{
    [SerializeField] bool IsUnscaled;
    float moveTime=0;
    float UTmoveTime=0;

    private void Start()
    {
        //transform.DOShakeRotation(1f);
        
    }

    private void Update()
    {
        if (IsUnscaled)
        {
            UTCubeMove();
        }
        else if (!IsUnscaled)
        {
            CubeMove();
        }
    }

    public void CubeMove()
    {
        moveTime += Time.deltaTime;
        //transform.position = new Vector3(Mathf.Sin(moveTime) * 10f,0f,0f);
        transform.DOMove(new Vector3(Mathf.Sin(moveTime) * 10f, 0f, 0f), 1f);
    }

    public void UTCubeMove()
    {
        UTmoveTime += Time.unscaledDeltaTime;
        //sdtransform.position = new Vector3(Mathf.Sin(UTmoveTime) * 10f, -4f, 0f);
        transform.DOMove(new Vector3(Mathf.Sin(UTmoveTime) * 10f, -4f, 0f), 1f);
    }
}
