using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


// FIXME : 완성 후 전체적으로 리펙토링 및 수정 필요
public class StoryDirector : BaseUI
{
    // 불러올 데이터가 있는 곳
    [SerializeField] StoryDirectingData storyData;

    [SerializeField] Image standingImagePrefab;

    // 추가적으로 컨트롤이 필요할 수 있는 박스에 대한 선언(당장 안써서 주석처리)
    [Header("텍스트 박스")]
    [SerializeField] GameObject nameBox;
    // [SerializeField] GameObject dialogueBox;

    [Header("텍스트")]
    // 불러온 데이터를 적용 시킬 곳
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;

    [Header("연출")]
    [SerializeField] RectTransform standingImageParent;
    [SerializeField] RawImage backGroundImage;

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

    private Image[] actors;

    // [SerializeField]  bool isAuto = false;
    // private int autoCounter = 0;


    private void Start()
    {
        if (storyData != null)
        {
            SetDirectionData(storyData);
        }

        // 시작할때 첫 대사
        StoryPrint(dialogueCounter);
    }

    protected override void Awake()
    {
        base.Awake();
        clickAction = GameManager.Input.actions["Touch"];
    }

    private void OnEnable()
    {
        clickAction.started += ClickAction_Started;
    }

    private void OnDisable()
    {
        clickAction.started -= ClickAction_Started;
    }

    private void OnDestroy() => ClearActors();

    public void SetDirectionData(StoryDirectingData storyData)
    {
        this.storyData = storyData;
        maxDialogueCounter = storyData.Dialogues.Length;

        ClearActors();
        actors = new Image[storyData.StandingImages.Length];
        foreach (StoryDirectingData.StandingImage actor in storyData.StandingImages)
        {
            actors[actor.ActorId] = Instantiate(standingImagePrefab, standingImageParent);
            actors[actor.ActorId].sprite = actor.ImageSprite;
            actors[actor.ActorId].SetNativeSize();
            actors[actor.ActorId].gameObject.SetActive(false);

        }
    }

    private void ClearActors()
    {
        if (actors != null)
        {
            foreach (Image actor in actors)
            {
                Destroy(actor.gameObject);
            }

            actors = null;
        }
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
        // 화자 출력
        nameText.text = storyData.Dialogues[nameCount].Speaker;
        
        // 대사 출력
        dialogueText.text = "";     // 앞전 스크립트 초기화용
        playingDialougeTween = dialogueText.DOText(storyData.Dialogues[nameCount].Script,1,true,ScrambleMode.None);
        playingDialougeTween.onComplete += () => playingDialougeTween = null; // 출력 완료시 참조 비우기

        // 사운드 출력
        if (null != storyData.Dialogues[nameCount].Bgm)
            GameManager.Sound.PlayBGM(storyData.Dialogues[nameCount].Bgm);

        if (null != storyData.Dialogues[nameCount].Sfx)
            GameManager.Sound.PlaySFX(storyData.Dialogues[nameCount].Sfx);

        if (null != storyData.Dialogues[nameCount].BackgroundSprite)
        {
            // TODO: 배경 변경 연출 추가
            backGroundImage.texture = storyData.Dialogues[nameCount].BackgroundSprite.texture;
        }

        foreach (StoryDirectingData.TransitionInfo transition in storyData.Dialogues[nameCount].Transitions)
        {
            Image actor = actors[transition.StandingImageId];

            // TODO: 출현/퇴장 연출
            actor.gameObject.SetActive(transition.Active);

            actor.transform.localScale = transition.Flip ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            actor.color = new Color(transition.ColorMultiply, transition.ColorMultiply, transition.ColorMultiply);

            actor.rectTransform.anchorMin = actor.rectTransform.anchorMax = transition.Position;
        }
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
        Destroy(this.gameObject, 3f);
    }
}