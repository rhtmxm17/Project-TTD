using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StoryDirector))]
public class StoryDirectorMenu : MonoBehaviour
{
    [SerializeField] Button skipButton;
    [SerializeField] Button hideButton;
    [SerializeField] Button logButton;
    [SerializeField] Button autoButton;
    [SerializeField] Button menuButton;
    [SerializeField] SkipWindow skipWindow;
    [SerializeField] HideWindow hideWindow;
    [SerializeField] MenuWindow menuWindow;

    [Space]
    [SerializeField] RectTransform settingPopupPrefab;
    [SerializeField] Material autoButtonShaderMaterial;

    [System.Serializable]
    private class SkipWindow
    {
        public GameObject GameObject;
        public Button BackGroundButton;
        public TMP_Text SynopsisText;
        public Button CancelButton;
        public Button ConfirmButton;

        public void OpenPopup() => GameObject.SetActive(true);
        public void ClosePopup() => GameObject.SetActive(false);
    }

    [System.Serializable]
    private class HideWindow
    {
        public GameObject HideTargetGameObject;
        public Button BackGroundButton;

        public void Hide()
        {
            HideTargetGameObject.SetActive(false);
            BackGroundButton.gameObject.SetActive(true);
        }
        public void Appear()
        {
            HideTargetGameObject.SetActive(true);
            BackGroundButton.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    private class MenuWindow
    {
        public GameObject GameObject;
        public Button BackGroundButton;
        public Button HelpButton;
        public Button SettingButton;
        public Button ExitButton;

        public void OpenPopup() => GameObject.SetActive(true);
        public void ClosePopup() => GameObject.SetActive(false);
    }


    private StoryDirector director;
    private Image autoButtonImage;

    private void Awake()
    {
        director = GetComponent<StoryDirector>();
        autoButtonImage = autoButton.GetComponent<Image>();
    }

    private void Start()
    {
        // 스킵 창
        skipButton.onClick.AddListener(skipWindow.OpenPopup);
        skipWindow.BackGroundButton.onClick.AddListener(skipWindow.ClosePopup);
        skipWindow.CancelButton.onClick.AddListener(skipWindow.ClosePopup);
        skipWindow.ConfirmButton.onClick.AddListener(director.OnComplete); // 스킵 확인시 스토리 종료시 수행할 작업 강제 실행

        // UI 숨기기
        hideButton.onClick.AddListener(hideWindow.Hide);
        hideWindow.BackGroundButton.onClick.AddListener(hideWindow.Appear);

        // 자동 재생 버튼
        autoButton.onClick.AddListener(ToggleAuto);

        // 메뉴 버튼
        menuButton.onClick.AddListener(menuWindow.OpenPopup);
        menuWindow.BackGroundButton.onClick.AddListener(menuWindow.ClosePopup);
        menuWindow.ExitButton.onClick.AddListener(() => Destroy(this.gameObject));
        menuWindow.SettingButton.onClick.AddListener(() => Instantiate(settingPopupPrefab, menuWindow.GameObject.transform));
    }

    private void ToggleAuto()
    {
        director.IsAutoPlayMode = !director.IsAutoPlayMode;
        if (director.IsAutoPlayMode)
        {
            autoButtonImage.material = autoButtonShaderMaterial;
        }
        else
        {
            autoButtonImage.material = null;
        }
    }
}
