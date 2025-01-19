using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using static StoryDirectingData;
using DG.Tweening.Plugins.Options;

public class StoryActor : MonoBehaviour
{
    public Sprite sprite { set => actorImage.sprite = value; }
    public Color color {  set => actorImage.color = value; }

    [SerializeField] RectTransform movementTransform;
    [SerializeField] RectTransform transitionTransform; // 이동에 동반되는 연출용 트랜스폼
    [SerializeField] Image actorImage;

    // 색상 및 페이드인/아웃
    private TweenerCore<Color, Color, ColorOptions> fadeTween;

    public void SetNativeSize() => actorImage.SetNativeSize();

    public void Transition(TransitionInfo transition)
    {
        // 색상 및 페이드인/아웃
        fadeTween.Complete(); // 이전 작업이 미완료일 경우 강제 완료
        fadeTween = actorImage.DOColor(new Color(
                transition.ColorMultiply,
                transition.ColorMultiply,
                transition.ColorMultiply,
                transition.Active ? 1f : 0f),
            transition.Time).SetUpdate(true);


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
            movementTransform.DOAnchorMin(transition.Position * 0.1f, transition.Time).SetUpdate(true);
            movementTransform.DOAnchorMax(transition.Position * 0.1f, transition.Time).SetUpdate(true);
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
                    .SetUpdate(true);
                break;
            case TransitionType.SHAKE:
                DOTween.Sequence()
                    .Append(transitionTransform.DOShakeAnchorPos(transition.Time, 100f, 10, 90f, false, false))
                    .SetUpdate(true);
                break;
            case TransitionType.SHAKE_HORIZONTAL:
                DOTween.Sequence()
                    .Append(transitionTransform.DOShakeAnchorPos(transition.Time, new Vector2(100f, 0f)))
                    .SetUpdate(true);
                break;
            case TransitionType.SPIRAL:
                DOTween.Sequence()
                    .Append(transitionTransform.DOSpiral(transition.Time, null, SpiralMode.ExpandThenContract, 1f, transition.Time * 0.01f))
                    .SetUpdate(true);
                break;
            case TransitionType.UPSIZE:
                DOTween.Sequence()
                    // 점차 확대 크기 임의 조정(현재 1.5배)(크기,확대속도)
                    .Append(transitionTransform.DOScale(2f, transition.Time))
                    .SetUpdate(true);
                break;
            case TransitionType.DOWNSIZE:
                DOTween.Sequence()
                    // 점차 축소 크기 임의 조정(현재 0.5배)(크기,축소속도)
                    .Append(transitionTransform.DOScale(0.5f,transition.Time))
                    .SetUpdate(true);
                break;
            default:
                Debug.LogWarning("정의되지 않은 트랜지션 타입");
                break;
        }
    }
}
