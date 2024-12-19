using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    // 대사 캐릭터 이름
    [SerializeField] public string name;
    // 대사 내용
    [SerializeField] public string[] contexts;
}

[System.Serializable]
public class DialogueEvent
{
    // 대화 이벤트 이름 = 후에 챕터의 이름 등으로 사용할 부분
    public string name;
    // X 줄부터 Y 줄의 대사를 가져옴
    public Vector2 line;
    // 대사를 여러개 가져올 상황 대비한 배열
    public Dialogue[] dialogue;
}

// TODO 
// 후에 데이터를 파서하고 데이터를 저장할 스크립트 작성
