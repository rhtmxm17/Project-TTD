using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// URP 카메라 기능 사용
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;
using static StoryDirectingData;

// FIXME : 완성 후 전체적으로 리펙토링 및 수정 필요
public class StoryDirector : BaseUI
{
    /// <summary>
    /// 스토리 출력 완료 후 실행할 동작
    /// </summary>
    public event UnityAction onStoryCompleted;

    public bool IsAutoPlayMode
    {
        get => isAutoPlayMode;
        set
        {
            if (isAutoPlayMode == value)
                return;

            isAutoPlayMode = value;

            // 오토 플레이 모드 진입 + 이번 다이얼로그 출력 완료 상태 
            if (isAutoPlayMode && playingDialougeTween == null)
            {
                // 다이얼로그 출력 완료 시점에 AutoPlayRoutine이 실행되므로
                // 이미 출력 완료 상태라면 오토플레이 진입시 루틴 시작 또는 다음 대사 출력 필요
                autoPlayRoutine = StartCoroutine(AutoPlayRoutine());
            }
        }
    }

    [SerializeField] StoryDirectingData testData;

    #region 초기화 필드
    [SerializeField] StoryActor standingImagePrefab;

    // 추가적으로 컨트롤이 필요할 수 있는 박스에 대한 선언(당장 안써서 주석처리)
    [Header("텍스트 박스")]
    [SerializeField] GameObject nameBox;
    // [SerializeField] GameObject dialogueBox;

    [Header("텍스트")]
    // 불러온 데이터를 적용 시킬 곳
    [SerializeField] TMP_Text locationText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;

    [Header("버튼")]
    [SerializeField] Button backGroundButton;

    [Header("연출")]
    [SerializeField] RectTransform standingImageParent;
    [SerializeField] RawImage backGroundImage;

    [Header("연출용 카메라")]
    [SerializeField] Camera directingCamera;
    #endregion 초기화 필드

    // 진행중인 연출 데이터
    private StoryDirectingData storyData;

    /// <summary>
    /// 현재 출력 진행중인 다이얼로그의 tweening<br/>
    /// null이면 출력 완료
    /// </summary>
    private DG.Tweening.Core.TweenerCore<string, string, DG.Tweening.Plugins.Options.StringOptions> playingDialougeTween = null;

    private int dialogueCounter = 0;
    private int maxDialogueCounter;

    private bool isAutoPlayMode = false;
    private Coroutine autoPlayRoutine = null;
    /// <summary>
    /// 오토플레이 모드시 대사 출력 완료 후 대기시간
    /// </summary>
    private YieldInstruction autoPlayDelay = new WaitForSeconds(1f);

    /// <summary>
    /// 등장 인물 목록
    /// </summary>
    private StoryActor[] actors;

    // [SerializeField]  bool isAuto = false;
    // private int autoCounter = 0;

    protected override void Awake()
    {
        DOTween.Init();

        base.Awake();
        backGroundButton.onClick.AddListener(OnClickForPlayNext);

        // 스탠딩 이미지 영역 설정
        standingImageParent.offsetMin = new Vector2(-standingImageParent.rect.height, 0f); 
    }

    private void OnDestroy() => ClearActors();

    [ContextMenu("테스트 데이터로 재생")]
    public void TestStart() => SetDirectionData(testData);

    public void SetDirectionData(StoryDirectingData storyData)
    {
        this.storyData = storyData;
        dialogueCounter = 0;
        maxDialogueCounter = storyData.Dialogues.Length;

        ClearActors();
        actors = new StoryActor[storyData.StandingImages.Length];
        foreach (StoryDirectingData.StandingImage actor in storyData.StandingImages)
        {
            actors[actor.ActorId] = Instantiate(standingImagePrefab, standingImageParent);
            actors[actor.ActorId].sprite = actor.ImageSprite;
            actors[actor.ActorId].color = new Color(1f, 1f, 1f, 0f);
            actors[actor.ActorId].SetNativeSize();

        }

        // 메인 카메라에 스토리 출력용 카메라 덮어 씌우기
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(this.directingCamera);

        StoryPrint(dialogueCounter);
    }

    private void ClearActors()
    {
        if (actors != null)
        {
            foreach (StoryActor actor in actors)
            {
                Destroy(actor.gameObject);
            }

            actors = null;
        }
    }

    private void OnClickForPlayNext()
    {
        if (playingDialougeTween == null)
        {
            PlayNextDialogue();
        }
        else
        {
            playingDialougeTween.Complete();
        }
    }

    /// <summary>
    /// 현재 다이얼로그 대사 출력 완료시 수행할 작업
    /// </summary>
    private void OnDialougeTweenComleted()
    {
        playingDialougeTween = null; // 출력 완료시 참조 비우기

        if (IsAutoPlayMode)
        {
            autoPlayRoutine = StartCoroutine(AutoPlayRoutine());
        }
    }

    private IEnumerator AutoPlayRoutine()
    {
        yield return autoPlayDelay;
        autoPlayRoutine = null;

        // 딜레이 후 '클릭시 동작' 수행 (클릭으로도 넘어갈 수 없는 상태라면 아무 일도 일어나지 않음)
        OnClickForPlayNext();
    }

    private void PlayNextDialogue()
    {
        TextCount();
        if (dialogueCounter == maxDialogueCounter)
        {
            StartCoroutine(StoryEndRoutine());
            return;
        }
        StoryPrint(dialogueCounter);
    }

    //public Vector2 CameraPosition;
    //public float CameraSize;

    // 실질적으로 스토리 텍스트를 출력할 함수
    private void StoryPrint(int nameCount)
    {
        StoryDirectingData.Dialogue dialogue = storyData.Dialogues[nameCount];

        // 장소 출력
        if (false == string.IsNullOrEmpty(dialogue.Loaction))
        {
            locationText.text = dialogue.Loaction;
        }

        // 화자 출력
        bool isNaration = string.IsNullOrEmpty(dialogue.Speaker);
        nameText.text = dialogue.Speaker;
        nameBox.SetActive(false == isNaration);

        // 대사 출력
        dialogueText.text = ""; // 앞전 스크립트 제거
        dialogueText.alignment = isNaration ? TextAlignmentOptions.Center : TextAlignmentOptions.Left; // 나레이션: 가운데, 대사: 왼쪽
        playingDialougeTween = dialogueText.DOText(dialogue.Script, dialogue.Script.Length * 0.05f * dialogue.timeMult, true, ScrambleMode.None).SetEase(Ease.Linear);
        playingDialougeTween.onComplete += OnDialougeTweenComleted;

        // 카메라 이동, 줌인아웃
        directingCamera.transform.DOLocalMove((dialogue.CameraPosition - new Vector2(10f, 5f)) * 2f, dialogue.CamereaTransitionTime);
        directingCamera.DOOrthoSize(dialogue.CameraSize * 10f, dialogue.CamereaTransitionTime);

        // 사운드 출력
        if (null != dialogue.Bgm)
            GameManager.Sound.PlayBGM(dialogue.Bgm);

        if (null != dialogue.Sfx)
            GameManager.Sound.PlaySFX(dialogue.Sfx);

        if (null != dialogue.BackgroundSprite)
        {
            // TODO: 배경 변경 연출 추가
            backGroundImage.texture = dialogue.BackgroundSprite.texture;
        }

        // 트랜지션 정보 적용
        foreach (StoryDirectingData.TransitionInfo transition in dialogue.Transitions)
        {
            actors[transition.StandingImageId].transform.SetAsLastSibling(); // 최근 트랜지션이 가장 앞에 노출되도록
            actors[transition.StandingImageId].Transition(transition);
        }

        // 이펙트 적용
        foreach (StoryDirectingData.EffectInfo effectInfo in dialogue.Effects)
        {
            StoryEffect effect = Instantiate(effectInfo.Effect, standingImageParent);
            effect.RectTransform.anchorMin = effect.RectTransform.anchorMax = effectInfo.Position * 0.1f;
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

    // 마지막 스크립트 출력 후 터치시 호출됨
    private IEnumerator StoryEndRoutine()
    {
        Debug.Log("스토리 출력 완료");
        backGroundButton.onClick.RemoveListener(OnClickForPlayNext);
        GameManager.Sound.StopBGM();

        yield return new WaitForSeconds(2f);

        // 스택된 카메라 제거
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Remove(this.directingCamera);

        // 등록된 스토리 종료시 호출될 액션 실행
        onStoryCompleted?.Invoke();

        Destroy(this.gameObject);
    }
}