using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour
{
    [SerializeField] Button backGroundButton;
    [SerializeField] Button closeButton;

    private void Awake()
    {
        backGroundButton.onClick.AddListener(CloseWindowWithSave);
        closeButton.onClick.AddListener(CloseWindowWithSave);
    }

    private void OnEnable()
    {
        UserSettingData.Instance.LoadSetting();
    }

    private void CloseWindowWithSave()
    {
        UserSettingData.Instance.SaveSetting();
        Destroy(this.gameObject);
    }
}
