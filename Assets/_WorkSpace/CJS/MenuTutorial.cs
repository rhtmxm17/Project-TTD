using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTutorial : MonoBehaviour
{
    enum TargetScene
    {
        None,
        Lobby,
        Shop,
        MyRoom,
        Story,
        Adventure,
    }

    [SerializeField] TargetScene targetScene;
    [SerializeField] RectTransform guidesParent;
    [SerializeField] Button foreGroundButton;

    private UserDataInt tutorialDone;
    private int currentGuid = 0;

    private void Awake()
    {
        tutorialDone = GameManager.UserData.PlayData.TutorialDone[(int)targetScene];
        
        // 이미 완료한 튜토리얼일 경우 파괴
        if (tutorialDone.Value != 0)
        {
            Destroy(this.gameObject);
            return;
        }

        guidesParent.GetChild(currentGuid).gameObject.SetActive(true);
        foreGroundButton.onClick.AddListener(ShowNextGuid);
    }

    private void ShowNextGuid()
    {
        // 더이상 가이드가 없을 경우 처리
        if (currentGuid == guidesParent.childCount - 1)
        {
            // 현재 튜토리얼 완료 사실 저장
            GameManager.UserData.StartUpdateStream()
                .SetDBValue(tutorialDone, 1)
                .Submit(null);

            Destroy(this.gameObject);
            return;
        }

        guidesParent.GetChild(currentGuid).gameObject.SetActive(false);
        currentGuid++;
        guidesParent.GetChild(currentGuid).gameObject.SetActive(true);

    }
}
