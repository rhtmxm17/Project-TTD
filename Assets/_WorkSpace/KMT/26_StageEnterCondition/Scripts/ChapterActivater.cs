using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterActivater : MonoBehaviour
{
    [SerializeField]
    Button button;
    [SerializeField]
    Image image;

    [Header("activation colors")]
    [SerializeField]
    Color activeColor;
    [SerializeField]
    Color inactiveColor;

    [Header("chapter's first stageData")]
    [SerializeField]
    StageData stageData;

    private void OnEnable()
    {
        if (stageData.IsOpened)
        {
            button.interactable = true;
            image.color = activeColor;
        }
        else
        {
            button.interactable = false;
            image.color = inactiveColor;
        }
    }

}
