using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRateTest : MonoBehaviour
{
    [SerializeField]
    int key;
    [SerializeField]
    int value;

    [SerializeField]
    int key2;
    [SerializeField]
    int value2;

    [ContextMenu("Adsfadsf")]
    public void asdf()
    {
        UserDataManager.Instance.StartUpdateStream()
            .SetDBDictionaryInnerValue(GameManager.UserData.PlayData.GoldDungeonClearRate, key.ToString(), value)
            .SetDBDictionaryInnerValue(GameManager.UserData.PlayData.GoldDungeonClearRate, key2.ToString(), value2)
            .Submit(null);
    }
}
