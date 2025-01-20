using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MusicBoxController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] Image image;
    private int num;

    private void Start()
    {
        PlayBGM();
    }

    private void PlayBGM()
    {
        SoundManager.Instance.PlayBGM(audioClips[Random.Range(0, audioClips.Length)]);
    }

    public void NextBGM()
    {
        num++;
        if(num >= audioClips.Length) num = 0;
        
        SoundManager.Instance.PlayBGM(audioClips[num]);
    }

    public void PreviousBGM()
    {
        num--;
        if(num< 0) num = audioClips.Length - 1;
        SoundManager.Instance.PlayBGM(audioClips[num]);
    }
}
