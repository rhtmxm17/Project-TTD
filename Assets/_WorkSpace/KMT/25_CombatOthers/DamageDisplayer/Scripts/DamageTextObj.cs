using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextObj : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;


    [SerializeField]
    Color damageColor;
    [SerializeField]
    Color HeadColor;

    RectTransform rect;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 데미지 숫자를 표기하는 텍스트 추가
    /// </summary>
    /// <param name="amount">데미지 크기[수치를 소수점 아래 두자리까지 표기]</param>
    /// <param name="isDamage">데미지여부 [ false인 경우 회복 효과 ]</param>
    /// <param name="position">시작 위치 [ 본인의 위치 ]</param>
    /// <param name="objPool">사용되고 복귀시킬 오브젝트 풀</param>
    public void SetDamageText(float amount, bool isDamage, Vector3 position, ObjectPool<DamageTextObj> objPool)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        text.text = amount < 0.00001f && isDamage? "막힘" : amount.ToString("0.##");
        text.color = isDamage ? damageColor : HeadColor;
        transform.position = position;
         
        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOLocalMove(new Vector3(rect.localPosition.x, rect.localPosition.y, rect.localPosition.z + 6), 2).SetEase(Ease.Linear));
        seq.Join(canvasGroup.DOFade(0, 2));
        seq.OnComplete(() => { gameObject.SetActive(false); objPool.BackItem(this); });
        seq.Play();
    }

}
