using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginCheckTest_kmt : MonoBehaviour
{

    [SerializeField]
    Button[] buttons;


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

    IEnumerator StartLogin(int dummyIdx)
    {
        Debug.Log("로그인 완료...");
        yield return new WaitForSeconds(1f);
        //yield return new WaitWhile(() => GameManager.UserData.Profile.Name.Value.Equals("이름 없음"));


        UserData.myUid = $"Dummy{dummyIdx}";
        UserData.myNickname = GameManager.UserData.Profile.Name.Value;

        SceneManager.LoadScene("FriendTest");
/*        UserDataManager.Instance.GetOtherUserProfileAsync("Dummy101", (profile) => {

            Debug.Log(profile.Name.Value);
            Debug.Log(profile.Level.Value);

        });*/
    }

}
