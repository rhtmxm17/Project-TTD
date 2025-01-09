using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameScene : MonoBehaviour
{
    public bool UseLegacyMain { get; set; } = false;

    [System.Serializable]
    private struct ChildUIField
    {
        public Button backGroundButton;
        public Button loginButton;
        public Button logoutButton;
        public GameObject loginPanel;
        public TMP_Text uidText;
        public TMP_Text versionText;
    }

    [SerializeField] ChildUIField childUIField;

    private IEnumerator Start()
    {
        GameManager.Instance.StartShortLoadingUI();
        Debug.Log("Firebase 초기화 대기중...");

        childUIField.loginButton.onClick.AddListener(PopLoginPanel);

        // Database 초기화 대기
        yield return new WaitWhile(() => GameManager.Database == null);
        // Auth 초기화 대기
        yield return new WaitWhile(() => GameManager.Auth == null);
        Debug.Log("Firebase 초기화 완료!");
        GameManager.Instance.StopShortLoadingUI();

        // 인증 정보 변경시 UI 갱신을 위한 구독
        GameManager.Auth.IdTokenChanged += OnAuthIdTokenChanged;
        UpdateLoginState();
    }

    private void OnDestroy()
    {
        GameManager.Auth.IdTokenChanged -= OnAuthIdTokenChanged;
    }

    private void OnAuthIdTokenChanged(object sender, System.EventArgs e) => UpdateLoginState();

    private void UpdateLoginState()
    {
        if (GameManager.Auth.CurrentUser == null)
        {
            childUIField.logoutButton.gameObject.SetActive(false); // 로그아웃 버튼 비활성화
            childUIField.loginButton.gameObject.SetActive(true); // 로그인 버튼 활성화

            // 빈 곳을 클릭시 로그인
            childUIField.backGroundButton.onClick.RemoveAllListeners();
            childUIField.backGroundButton.onClick.AddListener(PopLoginPanel);
        }
        else
        {
            childUIField.logoutButton.gameObject.SetActive(true); // 로그아웃 버튼 활성화
            childUIField.loginButton.gameObject.SetActive(false); // 로그인 버튼 비활성화

            // 로그인 성공시 로그인 창 닫기
            childUIField.loginPanel.gameObject.SetActive(false);

            GameManager.Instance.StartShortLoadingUI();
            Debug.Log("유저 ID 정보를 가져오는 중...");

            // currentUserDataRef가 갱신되지 않았을 수 있으므로 예외적으로 DatabaseReference를 직접 타서 UID를 가져온다
            BackendManager.AllUsersDataRef.Child($"{GameManager.Auth.CurrentUser.UserId}/Profile/Uid").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                GameManager.Instance.StopShortLoadingUI();

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning("UID 정보를 가져오는데 실패했습니다");
                    return;
                }

                long uid = 0;
                if (null == task.Result.Value || 0 == (uid = (long)task.Result.Value))
                {
                    childUIField.uidText.text = "UID를 찾을 수 없음";
                }
                else
                {
                    childUIField.uidText.text = $"UID: {uid}";
                }
            });

            // 빈 곳을 클릭시 게임 시작
            childUIField.backGroundButton.onClick.RemoveAllListeners();
            childUIField.backGroundButton.onClick.AddListener(StartGame);
        }
    }

    private void PopLoginPanel() => childUIField.loginPanel.SetActive(true);

    private void StartGame()
    {
        Debug.Log("유저 플레이 정보를 가져오는 중...");
        GameManager.Instance.StartShortLoadingUI();
        GameManager.UserData.onLoadUserDataCompleted.AddListener(CheckNextScene);
        GameManager.UserData.LoadUserData();
    }

    private void CheckNextScene()
    {
        Debug.Log("출석 확인 중...");
        DailyChecker.IsTodayFirstConnect((isFirst) =>
        {
            GameManager.Instance.StopShortLoadingUI();
            if (isFirst)
            {
                SceneManager.LoadScene("DailyBonusScene");
            }
            else
            {
                if (UseLegacyMain)
                    GameManager.Instance.LoadMainScene();
                else
                    GameManager.Instance.LoadLobbyScene();
            }

        });
    }
}
