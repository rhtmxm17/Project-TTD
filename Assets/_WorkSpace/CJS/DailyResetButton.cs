using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DailyResetButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(DailyResetCheat);
    }

    private void DailyResetCheat()
    {
        Debug.LogWarning("치트 기능 사용됨(일일 보상 재 수령)");
        SceneManager.LoadScene("DailyBonusScene");
    }
}
