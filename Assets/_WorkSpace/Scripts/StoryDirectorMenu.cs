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

    private StoryDirector director;

    private void Awake()
    {
        director = GetComponent<StoryDirector>();
    }

    private void Start()
    {
        // 스킵 창
        skipButton.onClick.AddListener(skipWindow.OpenPopup);
        skipWindow.BackGroundButton.onClick.AddListener(skipWindow.ClosePopup);
        skipWindow.CancelButton.onClick.AddListener(skipWindow.ClosePopup);
        skipWindow.ConfirmButton.onClick.AddListener(director.OnComplete); // 스킵 확인시 스토리 종료시 수행할 작업 강제 실행
    }

}
