using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UserProfileUI : BaseUI
{
    // 가져오고 써야할 데이터들
   private string nickName; // 닉네임
   private int UID; // UID?
   private int level; // 레벨
   private int CP; // 전투력 합계
   private string introduction; // 자기소개문구
   private int acquiredCharacter; // 캐릭터 보유 수
   private string stage; // 스테이지 클리어 진도
   private int lastRank; // 이전 레이드 순위
   private int bestRank; // 역대 최고 랭킹
   
   public UnityEvent OnChangeProfile;
    
    private void Start()
    {
        // 테스트용 더미 UID
        GameManager.UserData.TryInitDummyUserAsync(28, () =>
        {
            Debug.Log("완료");

            // 여기서 기존의 데이터를 불러오도록
            LoadData();
            // 불러온 데이터 UI 적용
            SetUI();
        });

        // UI 바인딩
        Init();
    }

    /*private void OnEnable()
    {
        // 데이터 변동이 있을때 데이터를 새로 갱신 
        // FIXME: 다른 데이터의 갱신이 없어서 이게 되는지 안되는지도 잘 모르겠음...이벤트 시스템을 잘 못써서...
        OnChangeProfile.AddListener(LoadData);
    }*/

    /// <summary>
    /// UI 바인딩
    /// </summary>
    private void Init()
    {
        // 프로필 닫기
        GetUI<Button>("Back").onClick.AddListener(() => ClosePopup("UserProfile"));
        // 소개 변경 창 열기
        GetUI<Button>("ChangeIntroduction").onClick.AddListener(() => OpenPopup("ChangeIntroPopup"));
        // 소개 변경 창 닫기 & 변경확인
        GetUI<Button>("Confirm").onClick.AddListener(() =>
        {
            ClosePopup("ChangeIntroPopup");
            SetIntroduction();
        });
        // 이름 변경 창 열기
        GetUI<Button>("ChangeNameButton").onClick.AddListener(() => OpenPopup("ChangeNamePopup"));
        // 이름변경 창 닫기 & 이름 변경
        GetUI<Button>("NameConfirm").onClick.AddListener(() =>
        {
            ClosePopup("ChangeNamePopup");
            SetName();
        });
    }

    /// <summary>
    /// 유저의 데이터를 DB에서 불러오는 작업
    /// </summary>
    private void LoadData()
    {
        // TODO: 불러올 데이터가 null 인경우에 대한 예외처리 필요
        
        // 데이터 불러오기
        nickName = GameManager.UserData.Profile.Name.Value;
        level = GameManager.UserData.Profile.Level.Value;
        introduction = GameManager.UserData.Profile.Introduction.Value;
        
        // TODO: 추가적인 데이터와 그에 따른 실시간 변동사항 고려
    }

    /// <summary>
    /// 유저의 정보를 UI에 적용하는 작업
    /// </summary>
    private void SetUI()
    {
        GetUI<TMP_Text>("Nickname").text = nickName;
        GetUI<TMP_Text>("Level").text = level.ToString();
        GetUI<TMP_Text>("Introduction").text = introduction;
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
        GetUI(_name).SetActive(true);
    }

    // 창닫기
    private void ClosePopup(string _name)
    {
        GetUI(_name).SetActive(false);
    }
}