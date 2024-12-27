using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_PlayerController : MonoBehaviour
{
    [Header("애니메이터 설정")]
    public Animator playerAnimator;
    private AnimatorOverrideController OverrideController;
    public Dictionary<string, List<AnimationClip>> StateAnimationPairs = new();
    public List<AnimationClip> Idle_List = new();
    public List<AnimationClip> Attack_List = new();


    [ContextMenu("dd")]
    public void TestMove()
    {
        Debug.Log(GetComponent<RectTransform>().anchoredPosition);
    }
    private void Update()
    {
        //애니메이터 오버라이드 컨트롤러를 확인하기 위한 공격
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerAnimator.SetTrigger("Attack");
        }
    }
}
