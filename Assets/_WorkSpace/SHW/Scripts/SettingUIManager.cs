using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingUIManager : BaseUI
{
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SetSettingUI();
    }

    private void SetSettingUI()
    {
        GetUI<Button>("GameSetting").onClick.AddListener(()=> OpenTap("GameSettingPanel"));
        GetUI<Button>("SoundSetting").onClick.AddListener(()=> OpenTap("SoundSettingPanel"));
        GetUI<Button>("EtcSetting").onClick.AddListener(()=> OpenTap("EtcSettingPanel"));
    }

    private void OpenTap(string _name)
    {
        // 지금 열려있는 패널들을 모두 닫고
        GetUI("GameSettingPanel").SetActive(false);
        GetUI("SoundSettingPanel").SetActive(false);
        GetUI("EtcSettingPanel").SetActive(false);
        // 이름이 들어간 버튼의 UI 활성화
        GetUI(_name).SetActive(true);
    }
}
