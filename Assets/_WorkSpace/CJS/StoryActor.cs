using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static StoryDirectingData;

public class StoryActor : MonoBehaviour
{
    public Sprite sprite { set => actorImage.sprite = value; }
    public Color color {  set => actorImage.color = value; }

    [SerializeField] RectTransform movementTransform;
    [SerializeField] RectTransform transitionTransform; // 이동에 동반되는 연출용 트랜스폼
    [SerializeField] Image actorImage;

    public void SetNativeSize() => actorImage.SetNativeSize();

    public void Transition(TransitionInfo transition)
    {
        // 색상 및 페이드인/아웃
        actorImage.DOColor(new Color(
                transition.ColorMultiply,
                transition.ColorMultiply,
                transition.ColorMultiply,
                transition.Active ? 1f : 0f),
            transition.Time);

        // 반전 및 크기
        transitionTransform.localScale = new Vector3(
            transition.Flip ? -transition.Scale : transition.Scale,
            transition.Scale,
            1);

        // 좌표 이동
        if (transition.Type == TransitionType.BLINK)
        {
            // 즉시 이동
            movementTransform.anchorMin = movementTransform.anchorMax = transition.Position * 0.1f;
        }
        else
        {
            // 선형 이동
            movementTransform.DOAnchorMin(transition.Position * 0.1f, transition.Time);
            movementTransform.DOAnchorMax(transition.Position * 0.1f, transition.Time);
        }

        // 트랜지션 연출
        switch (transition.Type)
        {
            case TransitionType.NONE:
            case TransitionType.BLINK:
            case TransitionType.NORMAL:
                // 기본 이동형 연출
                break;
            case TransitionType.BOUNCE:
                DOTween.Sequence()
                    .Append(transitionTransform.DOAnchorPos(Vector2.up * 100f, transition.Time * 0.25f))
                    .Append(transitionTransform.DOAnchorPos(Vector2.zero, transition.Time * 0.25f))
                    .Append(transitionTransform.DOAnchorPos(Vector2.up * 100f, transition.Time * 0.25f))
                    .Append(transitionTransform.DOAnchorPos(Vector2.zero, transition.Time * 0.25f))
                    ;
                break;
            default:
                Debug.LogWarning("정의되지 않은 트랜지션 타입");
                break;
        }
    }
}
