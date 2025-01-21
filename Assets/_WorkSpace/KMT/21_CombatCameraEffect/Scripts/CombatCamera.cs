using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCamera : MonoBehaviour
{
    [Header("Camera Lerp")]
    [SerializeField]
    Vector3 focusPos;
    [SerializeField]
    float focusViewSize;
    [SerializeField]
    [Range(0f, 10f)]
    float lerpSpeed;

    [Header("Camera Shake")]
    [SerializeField]
    Transform shakeCameraParent;
    [SerializeField]
    float shakeDistance;

    Vector3 originPos;
    float originViewSize;

    Coroutine shakeCoroutine = null;
    WaitForSeconds shakeOffset = new WaitForSeconds(0.02f);

    private void Awake()
    {
        float focusViewSizeDelta = Camera.main.orthographicSize - focusViewSize;

        originPos = transform.position;

        Camera.main.orthographicSize = ( Mathf.Max(((2.166666666666667f - (Screen.width / (float)Screen.height)) + 2.166666666666667f) / 2.166666666666667f, 1)) * 10;
        originViewSize = Camera.main.orthographicSize;
        focusViewSize = originViewSize - focusViewSizeDelta;

        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
        Debug.Log(Screen.width / (float)Screen.height);

    }

    public void Shake()
    {
        if (shakeCoroutine != null)
        { 
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
            shakeCameraParent.localPosition = Vector3.zero;
        }

        shakeCoroutine = StartCoroutine(ShakeCO());
    }

    public IEnumerator StartFocusCO()
    {
        yield return null;
        while (Vector3.SqrMagnitude(focusPos - transform.position) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, focusPos, lerpSpeed * Time.deltaTime);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, focusViewSize, lerpSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = focusPos;
        Camera.main.orthographicSize = focusViewSize;

    }
    public IEnumerator ReleaseFocusCO()
    {
        yield return null;
        while (Vector3.SqrMagnitude(originPos - transform.position) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, originPos, lerpSpeed * Time.deltaTime);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, originViewSize, lerpSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = originPos;
        Camera.main.orthographicSize = originViewSize;

    }


    IEnumerator ShakeCO()
    {
        yield return null;
        shakeCameraParent.localPosition = new Vector3(-shakeDistance, 0, shakeDistance);
        yield return shakeOffset;
        shakeCameraParent.localPosition = new Vector3(-shakeDistance, shakeDistance, -shakeDistance);
        yield return shakeOffset;
        shakeCameraParent.localPosition = Vector3.zero;
        yield return shakeOffset;
        shakeCameraParent.localPosition = new Vector3(shakeDistance, 0, shakeDistance);
        yield return shakeOffset;
        shakeCameraParent.localPosition = new Vector3(shakeDistance, 0, -shakeDistance);
        yield return shakeOffset;
        shakeCameraParent.localPosition = Vector3.zero;
    }

}
