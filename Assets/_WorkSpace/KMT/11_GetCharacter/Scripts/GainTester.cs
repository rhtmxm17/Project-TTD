using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainTester : MonoBehaviour
{
    [SerializeField]
    int id;
    IEnumerator Start()
    {
        yield return StartCoroutine(UserDataManager.InitDummyUser(id));
        Debug.Log("완료");
    }

    [SerializeField]
    int chId;
    [ContextMenu("Get")]
    public void Apply()
    {
        CharacterApplier.ApplyCharacter(chId);
    }

}
