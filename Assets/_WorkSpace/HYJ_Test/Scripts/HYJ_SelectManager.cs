using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UserDataManager;

public class HYJ_SelectManager : MonoBehaviour
{
    [SerializeField] public int curPos;
    [SerializeField] public int curUnitIndex;

    [Header("buttonList")]
    [SerializeField]
    List<Transform> buttonsTransformList;

    [Header("spriteTest")]
    [SerializeField]
    List<HYJ_PlayerController> spriteList;

    // 키 값은 위치 / 밸류 값은 유닛 고유번호;
    public Dictionary<int, int> battleInfo = new Dictionary<int, int>();

    public void LookLog()
    {
        DatabaseReference baseref = BackendManager.CurrentUserDataRef;
        Debug.Log("dd");
        baseref.Child("Batchs").SetValueAsync(null).ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("편성 초기화 실패");
                return;
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();

            Debug.Log("-------");
            foreach (var (key, value) in battleInfo)
            {
                updates[$"Batchs/{ key.ToString("D3")}"] = value;
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

    public void SetCharcterSprite(int posIdx, int charIdx)
    {
        var obj = Instantiate(spriteList[charIdx - 1], buttonsTransformList[posIdx]);
        obj.transform.localPosition = Vector3.zero;
    }

    public void RemoveCharacterSprite(int posIdx)
    {
        Destroy(buttonsTransformList[posIdx].GetComponentInChildren<HYJ_PlayerController>().gameObject);
    }

    public void ChangeTo(int fromIdx, int destIdx)
    {
        Destroy(buttonsTransformList[destIdx].GetComponentInChildren<HYJ_PlayerController>().gameObject);
        buttonsTransformList[fromIdx].GetComponentInChildren<HYJ_PlayerController>()
            .transform.SetParent(buttonsTransformList[destIdx].transform);

        HYJ_PlayerController[] transL = buttonsTransformList[destIdx].GetComponentsInChildren<HYJ_PlayerController>();
        RectTransform trans = transL[transL.Length - 1].GetComponent<RectTransform>();
        trans.anchoredPosition = Vector3.zero;
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
