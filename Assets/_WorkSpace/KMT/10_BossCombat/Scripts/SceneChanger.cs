using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    string destScene;

    void Start()
    {
        GameManager.UserData.TryInitDummyUserAsync(7, () =>
        {
            Debug.Log("완료");
            UserData.myUid = $"Dummy7";
            UserData.myNickname = GameManager.UserData.Profile.Name.Value;
            SceneManager.LoadScene(destScene);
        });
    }

    [ContextMenu("ChangeScene")]
    public void ChangeScene()
    { 
        SceneManager.LoadScene(destScene);
    }

}
