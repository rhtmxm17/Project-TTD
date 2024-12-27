using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    string destScene;

    IEnumerator Start()
    {
        yield return UserDataManager.InitDummyUser(7);
        Debug.Log("완료");
    }

    [ContextMenu("ChangeScene")]
    public void ChangeScene()
    { 
        SceneManager.LoadScene(destScene);
    }

}
