using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] AudioClip bgmClip;
    [SerializeField, Range(0f, 1f)] float clipScale = 1f; // 오디오클립별 개별 조정용도

    private void Start()
    {
        GameManager.Sound.PlayBGM(bgmClip, clipScale);
    }
}
