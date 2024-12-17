using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueData : MonoBehaviour
{
    // TODO
    // 파서해온 데이터를 저장할 부분
    // 혹은 불러올 부분
    
    // (테스트)
    // 해당 스토리의 챕터나 구별용 id 혹은 네임 설정
    [SerializeField] private string name;
    // 인스펙터 창에서 대사 설정
    [SerializeField] public Dialogue[] dialogues;
}
