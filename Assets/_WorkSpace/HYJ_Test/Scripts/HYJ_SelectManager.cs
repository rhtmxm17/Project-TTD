using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UserDataManager;

public class HYJ_SelectManager : MonoBehaviour
{
    [SerializeField] public int curPos;
    [SerializeField] public int curUnitIndex;

    // 키 값은 위치 / 밸류 값은 유닛 고유번호;
    public Dictionary<int, int> battleInfo = new Dictionary<int, int>();

    private void Awake()
    {
        //StartCoroutine(UserDataManager.InitDummyUser(7));
    }


    public void LookLog()
    {
        DatabaseReference baseref = BackendManager.CurrentUserDataRef;
        Debug.Log("dd");
        baseref.Child("Characters").SetValueAsync(null).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("편성 초기화 실패");
                return;
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();

            Debug.Log("-------");
            foreach (var (key, value) in battleInfo)
            {
                updates[$"Characters/{ key.ToString("D3")}"] = value;
                Debug.Log($"{key} : {value}");
            }
            Debug.Log("-------");

            baseref.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task => {

                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("추가 실패");
                    return;
                }

            });

        });

    }

    public void TestLog()
    {
        Debug.Log("-------");
        foreach (var (key, value) in battleInfo)
        {
            Debug.Log($"{key} : {value}");
        }
        Debug.Log("-------");
    }
}
