using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserProfileUI : BaseUI
{
    // 가져오고 써야할 데이터들
   [SerializeField] private string nickName; // 닉네임
   [SerializeField] private int UID; // UID?
   [SerializeField] private int level; // 레벨
   [SerializeField] private int CP; // 전투력 합계
   [SerializeField] private string introduction; // 자기소개문구
   [SerializeField] private int acquiredCharacter; // 캐릭터 보유 수
   [SerializeField] private string stage; // 스테이지 클리어 진도
   [SerializeField] private int lastRank; // 이전 레이드 순위
   [SerializeField] private int bestRank; // 역대 최고 랭킹
    
    private void Start()
       
    { 
        // 테스트용 더미 데이터
        // FIXME: 고장중..데이터 안올라감...주말에 고쳐오겠음...
        UserDataManager.InitDummyUser(28);
        
        // UI 바인딩
        Init();
        // 여기서 기존의 데이터를 불러오도록
        LoadData();
        // 불러온 데이터 UI 적용
        SetUI();
    }

    /// <summary>
    /// UI 바인딩
    /// </summary>
    private void Init()
    {
        // 프로필 닫기
        GetUI<Button>("Back").onClick.AddListener(() => ClosePopup("UserProfile"));
        // 소개 변경 창 열기
        GetUI<Button>("ChangeIntroduction").onClick.AddListener(() => OpenPopup("ChangeIntroPopup"));
        // 소개 변경 창 닫기
        GetUI<Button>("Confirm").onClick.AddListener(() => ClosePopup("ChangeIntroPopup"));
        // 소개문구 변경
        GetUI<Button>("Confirm").onClick.AddListener(() => SetIntroduction());
    }

    /// <summary>
    /// 유저의 데이터를 DB에서 불러오는 작업
    /// </summary>
    private void LoadData()
    {
        // TODO: 불러올 데이터가 null 인경우에 대한 예외처리 필요
        if (GameManager.UserData.Profile == null)
        {
            Debug.Log("데이터 없음!");
        }
        
        // 데이터 불러오기
        nickName = GameManager.UserData.Profile.Name.Value;
        level = GameManager.UserData.Profile.Level.Value;
        introduction = GameManager.UserData.Profile.Introduction.Value;
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
        // 인풋 필드의 내용을 introduction에 저장
        introduction = GetUI<TMP_InputField>("InputField").text ;
        GetUI<TMP_Text>("Introduction").text = introduction;
        // 저장한 데이터를 DB에 보냄
        // FIXME: 여기서 null래퍼뜸....
        GameManager.UserData.StartUpdateStream()
            .SetDBValue(GameManager.UserData.Profile.Introduction, introduction)
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