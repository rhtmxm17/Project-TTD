using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SoundSettingPanel : MonoBehaviour
{
    [SerializeField] AudioClip testClip;

    [Header("자식 UI")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Toggle masterMuteToggle;
    [SerializeField] Toggle bgmMuteToggle;
    [SerializeField] Toggle sfxMuteToggle;

    private AudioGroup targetGroup;
    private bool valueChanged = false;

    private InputAction touchAction;

    private void Awake()
    {
        touchAction = GameManager.Input.actions["Touch"];

        // 슬라이더 값은 백분율로 표시
        masterSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.MASTER) * 100f;
        bgmSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.BGM) * 100f;
        sfxSlider.value = GameManager.Sound.GetMixerScale(AudioGroup.SFX) * 100f;

        masterMuteToggle.isOn = GameManager.Sound.GetMute(AudioGroup.MASTER);
        bgmMuteToggle.isOn = GameManager.Sound.GetMute(AudioGroup.BGM);
        sfxMuteToggle.isOn = GameManager.Sound.GetMute(AudioGroup.SFX);

        masterSlider.onValueChanged.AddListener(value => { valueChanged = true; targetGroup = AudioGroup.MASTER; });
        bgmSlider.onValueChanged.AddListener(value => { valueChanged = true; targetGroup = AudioGroup.BGM; });
        sfxSlider.onValueChanged.AddListener(value => { valueChanged = true; targetGroup = AudioGroup.SFX; });

        masterMuteToggle.onValueChanged.AddListener(value => GameManager.Sound.SetMute(AudioGroup.MASTER, value));
        bgmMuteToggle.onValueChanged.AddListener(value => GameManager.Sound.SetMute(AudioGroup.BGM, value));
        sfxMuteToggle.onValueChanged.AddListener(value => GameManager.Sound.SetMute(AudioGroup.SFX, value));
    }

    private void OnEnable()
    {
        touchAction.canceled += TouchAction_canceled;
    }

    private void OnDisable()
    {
        touchAction.canceled -= TouchAction_canceled;
    }

    private void TouchAction_canceled(InputAction.CallbackContext obj)
    {
        Debug.Log("TouchAction_canceled");

        // 터지를 뗐을 때, 값 갱신이 확인되었다면 PointerUp 호출 (샘플 사운드)
        if (valueChanged)
            PointerUp();
    }

    private void SetAndPlay(AudioGroup audioGroup, float value)
    {
        Debug.Log($"설정값: {value}");
        GameManager.Sound.SetMixerScale(audioGroup, value * 0.01f); // 백분율 -> 0.0 ~ 1.0
        GameManager.Sound.PlayTestSound(audioGroup, testClip);
    }

    public void PointerUp()
    {
        // 마우스를 뗄 때에만 설정 및 재생
        // onValueChanged에서 하면 드래그시 연달아 재생됨(너무 귀 아픔)
        valueChanged = false;

        Slider target;
        switch (targetGroup)
        {
            case AudioGroup.MASTER:
                target = masterSlider;
                break;
            case AudioGroup.BGM:
                target = bgmSlider;
                break;
            case AudioGroup.SFX:
                target = sfxSlider;
                break;
            default:
                Debug.LogWarning("정의되지 않은 AudioGroup");
                return;
        }
        SetAndPlay(targetGroup, target.value);
    }
}
