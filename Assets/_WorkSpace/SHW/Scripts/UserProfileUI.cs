using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UserProfileUI : BaseUI
{
    [SerializeField] ProfileIconList profileIconList;
    [SerializeField] SetUserIcon iconSelectButtonPrefab;

    [Header("Child UI")]
    [SerializeField] OutskirtsUI outskirtsUI;
    [SerializeField] RectTransform iconSelectLayoutGroup;

    FirebaseAuth auth = BackendManager.Auth;

    // 가져오고 써야할 데이터들
    private string nickName; // 닉네임
    private int iconIndex;   // 아이콘 인덱스
    private int UID; // UID?
    private int level; // 레벨
    private float CP; // 전투력 합계
    private string introduction; // 자기소개문구
    private int acquiredCharacter; // 캐릭터 보유 수
    private StageData clearStage; // 스테이지 클리어 진도
    private int clearchapter; // 스테이지 클리어 진도
    private StageData clearStory; // 스토리 클리어 진도
    private int lastRank; // 이전 레이드 순위
    private int bestRank; // 역대 최고 랭킹

    public UnityEvent OnChangeProfile;
   
    // (임시) 아이콘 설정용 이미지
    private Sprite[] IconSprites => profileIconList.IconList;

    
    private void Start()
    {
        // UI 바인딩
        Init();

        if (BackendManager.CurrentUserDataRef == null)
        {
            // 테스트용 더미 UID
            GameManager.UserData.TryInitDummyUserAsync(28, () =>
            {
                Debug.Log("더미 로드");

                // 여기서 기존의 데이터를 불러오도록
                LoadData();
                // 불러온 데이터 UI 적용
                SetUI();
            });
        }
        else
        {
            // 여기서 기존의 데이터를 불러오도록
            LoadData();
            // 불러온 데이터 UI 적용
            SetUI();
        }

        for (int i = 0; i < profileIconList.IconList.Length; i++)
        {
            var buttonInstance = Instantiate(iconSelectButtonPrefab, iconSelectLayoutGroup);
            buttonInstance.IconSprite = profileIconList.IconList[i];
            buttonInstance.IconIndex = i;
        }
    }

    private void OnDestroy()
    {
        GameManager.UserData.Profile.IconIndex.onValueChanged -= OnProfileImageUpdated;
    }

    private void OnEnable()
    {
        LoadData();
    }

    /// <summary>
    /// UI 바인딩
    /// </summary>
    private void Init()
    {
        // 소개 변경 창 열기
        // GetUI<Button>("ChangeIntroduction").onClick.AddListener(() => OpenPopup("ChangeIntroPopup"));
        // 소개 변경 창 닫기 & 변경확인
        /*GetUI<Button>("Confirm").onClick.AddListener(() =>
        {
            ClosePopup("ChangeIntroPopup");
            SetIntroduction();
        });*/
        // 이름 변경& 아이콘 변경 창 여닫기
        GetUI<Button>("Edit Profile Invisible Button").onClick.AddListener(() => OpenPopup("CustomProfile"));
        GetUI<Button>("CustomProfileBack").onClick.AddListener(() => ClosePopup("CustomProfile"));
        // 이름 변경 창 열기
        GetUI<Button>("ChangeNameButton").onClick.AddListener(() => OpenPopup("ChangeNamePopup"));
        // 이름 변경 창 닫기
        GetUI<Button>("Change Name Background").onClick.AddListener(() => ClosePopup("ChangeNamePopup"));
        // 이름변경 창 닫기 & 이름 변경
        GetUI<Button>("NameConfirm").onClick.AddListener(() =>
        {
            ClosePopup("ChangeNamePopup");
            SetName();
        });
        // 대표캐릭터 변경팝업
        GetUI<Button>("ProfileIconButton").onClick.AddListener(() => OpenPopup("CharacterChangePopup"));
        GetUI<Button>("CloseChangeCharacter").onClick.AddListener(() => ClosePopup("CharacterChangePopup"));
        
        // DB랑 연관없이 인스펙터만으로 설정하는 부분 추가적인 작성이 필요햠
        // 대표 캐릭터 변경
    }

    /// <summary>
    /// 유저의 데이터를 DB에서 불러오는 작업
    /// </summary>
    private void LoadData()
    {
        // TODO: 불러올 데이터가 null 인경우에 대한 예외처리 필요
        
        // 데이터 불러오기
        nickName = GameManager.UserData.Profile.Name.Value;             // 닉네임
        level = GameManager.UserData.Profile.Level.Value;               // 레벨
        introduction = GameManager.UserData.Profile.Introduction.Value; // 자기소개(사용안함)
        
        // 클리어 정보
        // 일일던전
        // 스토리던
        for (int i = 1; i < 300; i++)
        {
            StageData stage = GameManager.TableData.GetStageData(i);
            if ((stage == null) || stage.ClearCount.Value == 0)
            {
                break;
            }
            clearStage = stage;
        }
        // 스토리 진행도
        for (int i = 10001; i < 10100; i++)
        {
            StageData stage = GameManager.TableData.GetStageData(i);
            if ((stage == null) || stage.ClearCount.Value == 0)
            {
                break;
            }

            clearStory = stage;
        }
        
        // 레이드 정보
        lastRank = GameManager.UserData.Profile.Rank.Value;              // 마지막 랭킹
        bestRank = GameManager.UserData.Profile.Rank.Value;              // 최고 랭킹
        
        // 보유 캐릭터
        acquiredCharacter = GameManager.UserData.HaveCharacterIdxList.Count;  // 획득 캐릭터 수 acquired
        // 총 전투력 cp
        for (int i = 0; i < acquiredCharacter; i++)
        {
            // 수치 맞는지 확인 필요..ㅠㅠ
            CP += GameManager.TableData.GetCharacterData(GameManager.UserData.HaveCharacterIdxList[i]).PowerLevel;
        }

        GameManager.UserData.Profile.IconIndex.onValueChanged += OnProfileImageUpdated;
        OnProfileImageUpdated(0);
        // TODO: 추가적인 데이터와 그에 따른 실시간 변동사항 고려
    }

    private void OnProfileImageUpdated(long _/*unused*/)
    {
        iconIndex = GameManager.UserData.Profile.IconIndex.Value;
        GetUI<Image>("ProfileIcon").sprite = IconSprites[iconIndex];
        GetUI<Image>("ProfileIconButton").sprite = IconSprites[iconIndex];
    }

    /// <summary>
    /// 유저의 정보를 UI에 적용하는 작업
    /// </summary>
    private void SetUI()
    {
        GetUI<TMP_Text>("Nickname").text = nickName;
        GetUI<TMP_Text>("Edit Window Name Text").text = nickName;
        GetUI<TMP_Text>("Level").text = level.ToString();
        //  GetUI<TMP_Text>("Introduction").text = introduction;
        GetUI<TMP_Text>("ClearStage").text = (clearStage == null) ?
            "클리어 한 스테이지 없음" :
            $"클리어 스테이지:{clearStage.ButtonName})";
        if(clearStory==null) GetUI<TMP_Text>("ClearStory").text = $"클리어 스토리:기록없음";
        else GetUI<TMP_Text>("ClearStory").text = $"클리어 스토리:{clearStory.ButtonName}";
        GetUI<TMP_Text>("BestRank").text = $"최고 랭킹:{bestRank.ToString()}";
        if(bestRank == 999) GetUI<TMP_Text>("BestRank").text = $"랭킹없음";
        GetUI<TMP_Text>("LastRank").text = $"최근 랭킹:{lastRank.ToString()}";
        if(lastRank == 999) GetUI<TMP_Text>("LastRank").text = $"랭킹없음";
        GetUI<TMP_Text>("CP").text = $"총전투력:{CP.ToString()}";
        GetUI<TMP_Text>("AcquiredCharacter").text = $"소유캐릭터 : {acquiredCharacter.ToString()}/9";
    }

    /// <summary>
    /// 자기소개를 변경하고 DB에 보내는 작업
    /// </summary>
    private void SetIntroduction()
    {
        if (GetUI<TMP_InputField>("InputField").text == "")
        {
            Debug.LogWarning("공백! 내용 입력 바람");
            return;
        }
        introduction = GetUI<TMP_InputField>("InputField").text ;
        // 인풋 필드의 내용을 introduction에 저장
        GetUI<TMP_Text>("Introduction").text = introduction;
        // 저장한 데이터를 DB에 보냄
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(GameManager.UserData.Profile.Introduction, introduction)
            .Submit(result =>
            {
                if (false == result)
                    Debug.Log($"요청 전송에 실패함");
            });
    }

    /// <summary>
    /// 유저의 이름을 변경하고 DB에 보내는 작업
    /// </summary>
    private void SetName()
    {
        if (GetUI<TMP_InputField>("NameInputField").text == "")
        {
            Debug.LogWarning("공백! 내용 입력 바람");
            return;
        }
        nickName = GetUI<TMP_InputField>("NameInputField").text ;
        GetUI<TMP_Text>("Nickname").text = nickName;
        GetUI<TMP_Text>("Edit Window Name Text").text = nickName;

        // 저장한 데이터를 DB에 보냄
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(GameManager.UserData.Profile.Name, nickName)
            .Submit(result =>
            {
                if (false == result)
                    Debug.Log($"요청 전송에 실패함");
            });
    }

    // 창열기
    private void OpenPopup(string _name)
    {
        GameObject popup = GetUI(_name);
        popup.SetActive(true);
        // outskirtsUI.UIStack.Push(popup);
    }

    // 창닫기
    private void ClosePopup(string _name)
    {
        GetUI(_name).SetActive(false);
    }
}