using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;

public class UIManager : BaseUI
{
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // 메인
        SetMainCanvas();
        // 스토리
        // 전투
        // 설정
        // 숙소
        // 상점
        // 캐릭터
        // 플레이어 정보
    }

    private void SetMainCanvas()
    {
        GetUI("Main");
        GetUI<Button>("CharacterButton").onClick.AddListener(() =>OpenUI("Character"));
    }

    /// <summary>
    /// UI들을 SetActive 해주는 함수. 
    /// </summary>
    /// <param name="_name">열고싶은 패널등의 이름을 적습니다.</param>
    public void OpenUI(string _name)
    {
        GetUI(_name).SetActive(true);
    }

    /// <summary>
    /// UI오브젝트의 패널을 닫습니다.setActive(false) 해줍니다.
    /// </summary>
    /// <param name="_name">닫고 싶은 패널등의 이름을 적습니다.</param>
    public void CloseUI(string _name)
    {
        GetUI(_name).SetActive(false);
    }
}
