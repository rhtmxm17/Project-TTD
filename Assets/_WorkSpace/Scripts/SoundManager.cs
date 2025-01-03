using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioGroup { MASTER, BGM, SFX }

public class SoundManager : SingletonBehaviour<SoundManager>
{
    public const int AudioGroupCount = 3;

    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    private static readonly string[] paramNames = { "Master", "BGM", "SFX" };

    private struct Volume
    {
        public bool isMute;
        public float scale;
    }

    private Volume[] mixerVolume = new Volume[AudioGroupCount];

    private void Awake()
    {
        RegisterSingleton(this);

    }

    private void Start()
    {
        // 음량 기본값 설정
        // 플레이어 로컬 설정 파일을 만든다면 여기서 입력

        for (int i = 0; i < AudioGroupCount; i++)
        {
            mixerVolume[i].scale = 1f;

        }

        UpdateMixer(AudioGroup.MASTER);
        UpdateMixer(AudioGroup.BGM);
        UpdateMixer(AudioGroup.SFX);
    }

    /// <summary>
    /// BGM을 재생
    /// </summary>
    /// <param name="clip">BGM으로 재생할 오디오 클립</param>
    /// <param name="volumeScale">음량 배율</param>
    public void PlayBGM(AudioClip clip, float volumeScale = 1f)
    {
        if (bgmSource.clip == clip) return;
        StopBGM();
        bgmSource.clip = clip;
        bgmSource.volume = volumeScale;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 효과음을 재생, 거리의 영향을 받는 효과음은 매니저를 통하지 말고 따로 재생 필요
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="volumeScale">음량 배율</param>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f) => sfxSource.PlayOneShot(clip, volumeScale);

    /// <summary>
    /// 해당 오디오 그룹에 대한 전역 음량 스케일을 설정<br/>
    /// Playe 메서드의 매개변수 volumeScale과는 별개<br/>
    /// 0~1 입력값이 -20dB ~ 0dB로 조정됨
    /// </summary>
    /// <param name="group">그룹</param>
    /// <param name="scale">음량(0~1 권장)</param>
    public void SetMixerScale(AudioGroup group, float scale)
    {
        mixerVolume[(int)group].scale = scale;
        UpdateMixer(group);
    }

    public float GetMixerScale(AudioGroup group)
    {
        return mixerVolume[(int)group].scale;
    }

    public void SetMute(AudioGroup group, bool isMute)
    {
        mixerVolume[(int)group].isMute = isMute;
        UpdateMixer(group);
    }

    public bool GetMute(AudioGroup group)
    {
        return mixerVolume[(int)group].isMute;
    }

    private void UpdateMixer(AudioGroup group)
    {
        float scale;
        if (mixerVolume[(int)group].isMute)
            scale = -80f;
        else
            scale = (mixerVolume[(int)group].scale - 1f) * 20f;

        mixer.SetFloat(paramNames[(int)group], scale);
    }

    public void PlayTestSound(AudioGroup group, AudioClip clip)
    {
        switch (group)
        {
            case AudioGroup.MASTER:
                bgmSource.PlayOneShot(clip);
                sfxSource.PlayOneShot(clip);
                return;
            case AudioGroup.BGM:
                bgmSource.PlayOneShot(clip);
                return;
            case AudioGroup.SFX:
                sfxSource.PlayOneShot(clip);
                return;
            default:
                Debug.LogWarning("잘못된 AudioGroup");
                return;
        }
    }
}
