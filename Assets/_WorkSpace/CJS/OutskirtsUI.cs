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
    /// 캐릭터 창 빠른 이동 버튼
    /// </summary>
    public Button CharactersButton => childUIField.charactersButton;

    /// <summary>
    /// 엄적 창 빠른 이동 버튼
    /// </summary>
    public Button AchievementButton => childUIField.achievementButton;

    /// <summary>
    /// 스토리 창 빠른 이동 버튼
    /// </summary>
    public Button StoryButton => childUIField.storyButton;

    /// <summary>
    /// 상점 창 빠른 이동 버튼
    /// </summary>
    public Button ShopButton => childUIField.shopButton;

    /// <summary>
    /// 나만의 공간 빠른 이동 버튼
    /// </summary>
    public Button MyRoomButton => childUIField.myRoomButton;

    /// <summary>
    /// 모험 창 빠른 이동 버튼
    /// </summary>
    public Button AdvantureButton => childUIField.advantureButton;

    [System.Serializable]
    private struct ChildUIField
    {
        public TMP_Text title;
        public Button returnButton;
        public Button homeButton;
        public Button quickMoveMenuButton;
        public LayoutGroup quickMoveLayout;
        public Button charactersButton;
        public Button achievementButton;
        public Button storyButton;
        public Button shopButton;
        public Button myRoomButton;
        public Button advantureButton;
    }
    [SerializeField] ChildUIField childUIField;

    private bool quickMoveToggleState = false;

    private void Awake()
    {
        childUIField.quickMoveLayout.gameObject.SetActive(false);
        QuickMoveMenuButton.onClick.AddListener(ToggleQuickMove);

        ReturnButton.onClick.AddListener(OnReturnButtonClicked);
        // 다른 팝업으로 가려졌을때는 동작하지 않아야 함
        //GameManager.Input.actions["Cancel"].started += OnReturnButtonClicked;

        HomeButton.onClick.AddListener(OnHomeButtonClicked);
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
        GameManager.Instance.LoadMainScene();
    }
}
