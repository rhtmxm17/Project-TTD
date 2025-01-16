using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    [System.Serializable]
    private struct ChildUIField
    {
        public Button profileButton;
        public Button settingButton;
        [EnumNamedArray(typeof(MenuType))]
        public List<Button> menuSceneButtons;
    }
    [SerializeField] ChildUIField childUIField;

    private void Awake()
    {
        childUIField.profileButton.onClick.AddListener(() => GameManager.Instance.LoadMenuScene(MenuType.PROFILE));
        childUIField.settingButton.onClick.AddListener(() => GameManager.OverlayUIManager.PopupSettingWindow());

        for (int i = 0; i < childUIField.menuSceneButtons.Count; i++)
        {
            MenuType menu = (MenuType)i;
            childUIField.menuSceneButtons[i].onClick.AddListener(() => GameManager.Instance.LoadMenuScene(menu));
        }
    }
}
