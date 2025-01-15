using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 화면 상단의 뒤로가기, 홈 버튼, 재화 표시 등 메뉴바와 우측 하단에 빠른 이동 버튼으로 구성된 UI<br/>
/// 
/// </summary>
public class OutskirtsUI : MonoBehaviour
{
    /// <summary>
    /// 상단 메뉴에 표시되는 현재 창 이름
    /// </summary>
    public TMP_Text Title => childUIField.title;

    /// <summary>
    /// 뒤로 가기 버튼으로 닫힐 UI 스택, 씬 안에서 뒤로가기로 나갈 수 있는 UI를 띄울 떄에 이 스택에 추가할것
    /// </summary>
    public readonly Stack<GameObject> UIStack = new Stack<GameObject>();

    /// <summary>
    /// 뒤로 가기 버튼
    /// </summary>
    public Button ReturnButton => childUIField.returnButton;

    /// <summary>
    /// 홈 버튼
    /// </summary>
    public Button HomeButton => childUIField.homeButton;

    /// <summary>
    /// 우하단 빠른 이동 메뉴를 여는 버튼
    /// </summary>
    public Button QuickMoveMenuButton => childUIField.quickMoveMenuButton;

    /// <summary>
    /// 빠른 이동 버튼 리스트
    /// </summary>
    public List<Button> MenuSceneButtons => childUIField.menuSceneButtons;

    [System.Serializable]
    private struct ChildUIField
    {
        public TMP_Text title;
        public Button returnButton;
        public Button homeButton;
        public Button quickMoveMenuButton;
        public LayoutGroup quickMoveLayout;
        [EnumNamedArray(typeof(MenuType))]
        public List<Button> menuSceneButtons;
    }
    [SerializeField] ChildUIField childUIField;

    private bool quickMoveToggleState;

    private void Awake()
    {
        quickMoveToggleState = childUIField.quickMoveLayout.gameObject.activeSelf;
        QuickMoveMenuButton.onClick.AddListener(ToggleQuickMove);

        ReturnButton.onClick.AddListener(OnReturnButtonClicked);
        // 다른 팝업으로 가려졌을때는 동작하지 않아야 함
        //GameManager.Input.actions["Cancel"].started += OnReturnButtonClicked;

        // 홈 버튼에 씬 전환 등록
        HomeButton.onClick.AddListener(OnHomeButtonClicked);

        // 각 빠른이동 버튼에 해당 씬 전환 함수 등록
        for (int i = 0; i < childUIField.menuSceneButtons.Count; i++)
        {
            MenuType menu = (MenuType)i;
            childUIField.menuSceneButtons[i].onClick.AddListener(() => GameManager.Instance.LoadMenuScene(menu));
        }
    }

    private void ToggleQuickMove()
    {
        if (quickMoveToggleState)
        {
            quickMoveToggleState = false;
            childUIField.quickMoveLayout.gameObject.SetActive(false);
        }
        else
        {
            quickMoveToggleState = true;
            childUIField.quickMoveLayout.gameObject.SetActive(true);
        }
    }

    //private void OnReturnButtonClicked(InputAction.CallbackContext context) => OnReturnButtonClicked();

    private void OnReturnButtonClicked()
    {
        if (UIStack.TryPop(out GameObject closingUI))
        {
            // 등록된 닫을 UI가 있다면 닫는다
            closingUI.SetActive(false);
        }
        else
        {
            // 현재 씬의 최상위 UI라면 홈 버튼과 동일
            OnHomeButtonClicked();
        }
    }

    private void OnHomeButtonClicked()
    {
        GameManager.Instance.LoadLobbyScene();
    }
}
