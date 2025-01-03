using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginCheckTest_kmt : MonoBehaviour
{

    [SerializeField]
    Button[] buttons;

    private void Awake()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int dummyIdx = i + 1000;
            buttons[i].onClick.AddListener(() => OnClick(dummyIdx));
        }
    }

    public void OnClick(int dummyIdx)
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }

        Debug.Log($"로그인 시작... {dummyIdx}번...");

        GameManager.UserData.TryInitDummyUserAsync(dummyIdx, () =>
        {
            StartCoroutine(StartLogin(dummyIdx));
        });
    }

    [SerializeField]
    string destScene;

    IEnumerator StartLogin(int dummyIdx)
    {
        Debug.Log("로그인 완료...");
        yield return new WaitForSeconds(1f);
        //yield return new WaitWhile(() => GameManager.UserData.Profile.Name.Value.Equals("이름 없음"));


        //UserData.myUid = $"Dummy{dummyIdx}";
        //UserData.myNickname = GameManager.UserData.Profile.Name.Value;

        DailyChecker.IsTodayFirstConnect((isFirst) => {
            Debug.Log("체크끝");
            if (isFirst)
            {
                SceneManager.LoadScene("DailyBonusScene");
            }
            else
            {
                SceneManager.LoadScene(destScene);
            }

    });

    }

}
