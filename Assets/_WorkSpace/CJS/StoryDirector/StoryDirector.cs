using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


// FIXME : 완성 후 전체적으로 리펙토링 및 수정 필요
public class StoryDirector : BaseUI
{

    // 스토리가 끝나면 스토리 프리팹 삭제
    [SerializeField] GameObject storyPrefab;

    // 불러올 데이터가 있는 곳
    [SerializeField] StoryDirectingData storyData;

    // 추가적으로 컨트롤이 필요할 수 있는 박스에 대한 선언(당장 안써서 주석처리)
    [Header("텍스트 박스")]
    [SerializeField] GameObject nameBox;
    // [SerializeField] GameObject dialogueBox;

    [Header("텍스트")]
    // 불러온 데이터를 적용 시킬 곳
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;

    // 화면 터치 인풋
    private InputAction clickAction;

    /// <summary>
    /// 현재 출력 진행중인 다이얼로그의 tweening<br/>
    /// null이면 출력 완료
    /// </summary>
    private DG.Tweening.Core.TweenerCore<string, string, DG.Tweening.Plugins.Options.StringOptions> playingDialougeTween = null;

    // private Dialogue[] dialogues;
    // 대사의 카운트를 늘릴 변수
    private int dialogueCounter = 0;
    private int maxDialogueCounter;

    // [SerializeField]  bool isAuto = false;
    // private int autoCounter = 0;


    private void Start()
    {
        clickAction = GameManager.Input.actions["Touch"];
        clickAction.started += ClickAction_Started;

        maxDialogueCounter = storyData.Dialogues.Length;

        // 시작할때 첫 대사
        StoryPrint(dialogueCounter);
    }

    private void ClickAction_Started(InputAction.CallbackContext obj)
    {
        // 다른 버튼이 선택되었을 경우 무시
        if (EventSystem.current.currentSelectedGameObject == true)
            return;

        if (playingDialougeTween == null)
        {
            PlayNextDialogue();
        }
        else
        {
            playingDialougeTween.Complete();
        }
    }

    private void PlayNextDialogue()
    {
        TextCount();
        if (dialogueCounter == maxDialogueCounter)
        {
            StoryEnd();
            return;
        }
        StoryPrint(dialogueCounter);
    }

    // 실질적으로 스토리 텍스트를 출력할 함수
    private void StoryPrint(int nameCount)
    {
        nameText.text = storyData.Dialogues[nameCount].Speaker;
        
        dialogueText.text = "";     // 앞전 스크립트 초기화용
        playingDialougeTween = dialogueText.DOText(storyData.Dialogues[nameCount].Script,1,true,ScrambleMode.None);
        playingDialougeTween.onComplete += () => playingDialougeTween = null; // 출력 완료시 참조 비우기
    }

    // 텍스트 카운터를 늘려줄 함수
    private void TextCount()
    {
        if (dialogueCounter < maxDialogueCounter)
        {
            dialogueCounter++;
        }
    }

    // 스토리가 끝났을 때 (임시) 자동으로 창을 닫는 함수
    private void StoryEnd()
    {
        // TODO : 스토리 끝난 이후 할 행동에 대한 구현
        // 기획 의도에 따라 자동으로 다음 스토리로 넘어갈지, 혹은 전투씬에서 스토리를 부를 경우 창을 바로 없앨지 등에 고민
        Debug.Log("스토리 출력 끝");
        nameBox.SetActive(false);   
        dialogueText.text = "-다음이시간에-";
        // (임시)스토리가 끝나면 창을 자동으로 닫음
        Destroy(storyPrefab,3f);
    }

    // 스토리 진행시 캐릭터의 이미지를 변경할 함수
    private void ChangeCharacterImage()
    {
        // FIXME : 나중에는 전투에서 캐릭터 이미지 불러오듯이 수정
        // 이름 텍스트와 같은 캐릭터의 이미지 출력(임시)
        
        
    }

}