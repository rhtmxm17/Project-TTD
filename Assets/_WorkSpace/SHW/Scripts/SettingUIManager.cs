using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        GetUI<Button>("SettingBackButton").onClick.AddListener(()=>CloseUI("Setting"));
        
        GetUI<Button>("GameSetting").onClick.AddListener(()=> OpenTap("GameSettingPanel"));
        GetUI<Button>("SoundSetting").onClick.AddListener(()=> OpenTap("SoundSettingPanel"));
        GetUI<Button>("EtcSetting").onClick.AddListener(()=> OpenTap("EtcSettingPanel"));
        
        // 사운드 셋팅용
         GetUI<Slider>("MainSoundSlider").onValueChanged.AddListener(ChangedMasterSound);
         GetUI<Slider>("BGMSoundSlider").onValueChanged.AddListener(ChangedBGM);
         GetUI<Slider>("SFXSoundSlider").onValueChanged.AddListener(ChangedSFX);
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

    private void CloseUI(string _name)
    {
        GetUI(_name).SetActive(false);
    }

    private void ChangedMasterSound(float _value)
    {
        // 사운드 매니저 속 setmixerScale 때문에 기본 0~20으로 조정
        // TODO:뮤트 버튼을 따로 만들어야 하나?
        GameManager.Sound.SetMixerScale(AudioGroup.MASTER, _value);
    }
    
    private void ChangedBGM(float _value)
    {
        GameManager.Sound.SetMixerScale(AudioGroup.BGM, _value);
    }
    
    private void ChangedSFX(float _value)
    {
        GameManager.Sound.SetMixerScale(AudioGroup.SFX, _value);
    }
}
