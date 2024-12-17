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

    public void AnimatorSetting()
    {
        Animator animator = playerAnimator;
        OverrideController = new AnimatorOverrideController();
        OverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

        // 모든 애니메이션 클립 가져오기
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            // 복제된 클립으로 오버라이드
            OverrideController[clip.name] = clip;
        }

        animator.runtimeAnimatorController = OverrideController;
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            var stateText = state.ToString();
            StateAnimationPairs[stateText] = new List<AnimationClip>();
            switch (stateText)
            {
                case "IDLE":
                    StateAnimationPairs[stateText] = Idle_List;
                    break;
                
                case "ATTACK":
                    StateAnimationPairs[stateText] = Attack_List;
                    break;
               
            }
        }
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
