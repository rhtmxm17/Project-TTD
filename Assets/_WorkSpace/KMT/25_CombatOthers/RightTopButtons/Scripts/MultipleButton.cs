using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultipleButton : MonoBehaviour
{
    float[] timescales = {1, 2, 3};
    string[] icons = { "▶", "▶▶", "▶▶▶" };
    int idx;

    [SerializeField]
    TextMeshProUGUI buttonText;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
        idx = 0;
    }

    void OnClicked()
    {
        if (StageManager.Instance.IsCombatEnd)
        {
            Time.timeScale = 1;
            return;
        }

        idx = (idx + 1) % icons.Length;
        Time.timeScale = timescales[idx];
        buttonText.text = icons[idx];
    }
}
