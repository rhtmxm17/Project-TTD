using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StoryActor : MonoBehaviour
{
    public Sprite sprite { set => actorImage.sprite = value; }
    public Color color {  set => actorImage.color = value; }

    [SerializeField] RectTransform movementTransform;
    [SerializeField] RectTransform transitionTransform; // 이동에 동반되는 연출용 트랜스폼
    [SerializeField] Image actorImage;

    public void SetNativeSize() => actorImage.SetNativeSize();

    public void Transition(StoryDirectingData.TransitionInfo transition)
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
        movementTransform.DOAnchorMin(transition.Position * 0.1f, transition.Time);
        movementTransform.DOAnchorMax(transition.Position * 0.1f, transition.Time);
    }
}
