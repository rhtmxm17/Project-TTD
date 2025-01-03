using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GainTester : MonoBehaviour
{
    [SerializeField]
    int id;
    void Start()
    {
        GameManager.UserData.TryInitDummyUserAsync(id, () =>
        {
            Debug.Log("완료");
        });
    }

    [SerializeField]
    int chId;
    [ContextMenu("Get")]
    public void Apply()
    {
        CharacterApplier.ApplyCharacter(chId);
    }

}
