using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 배경 버튼과 확인 버튼에 기본적으로 자괴가 붙어있는 팝업
/// </summary>
public class SimpleInfoPopup : MonoBehaviour
{
    public Button BackgroundButton => initFields.backgroundButton;
    public Button SubmitButton => initFields.submitButton;
    public TMP_Text Title => initFields.title;
    public TMP_Text Description => initFields.description;
    public Image Image => initFields.image;

    [System.Serializable]
    private struct InitFields
    {
        public Button backgroundButton;
        public Button submitButton;
        public TMP_Text title;
        public TMP_Text description;
        public Image image;
    }

    [SerializeField] InitFields initFields;

    private void Awake()
    {
        BackgroundButton.onClick.AddListener(() => Destroy(this.gameObject));
        SubmitButton.onClick.AddListener(() => Destroy(this.gameObject));
    }
}
