using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObjects/StoryDirectingData")]
public class StoryDirectingData : ScriptableObject, ICsvSheetParseable
{
    [System.Serializable]
    public struct Dialogue
    {
        public string Speaker;
        public string Script;

        // 해당 다이얼로그 재생과 함께 진행되는 갱신사항들
        // null이라면 해당 내용은 갱신하지 않음을 의미
        public AudioClip Bgm;
        public AudioClip Sfx;
        public Sprite BackgroundSprite;
        public TransitionInfo[] Transitions; // 스탠딩 이미지 갱신사항
    }

    [System.Serializable]
    public struct TransitionInfo
    {
        public int StandingImageId; // 어느 이미지를 갱신할 것인지
        public bool Active; // 출현 또는 숨기기
        public bool Flip;
        public float ColorMultiply; // 1(일반)~0(어두움)
        public Vector2 Position; // 유효 영역 기준, 좌하단을 (0, 0) 우상단을 (1, 1)로 하는 위치 좌표
    }

    [System.Serializable]
    public struct StandingImage
    {
        public int Id; // 해당 스토리에서 사용되는 ID(직렬화된 배열 인덱스)
        public Sprite ImageSprite;
    }

    [SerializeField] Dialogue[] dialogues;
    public Dialogue[] Dialogues => dialogues;

    [SerializeField] StandingImage[] standingImages;
    public StandingImage[] StandingImages => standingImages;

#if UNITY_EDITOR
    private enum Column
    {
        ID,
        SPEAKER,
        SCRIPT,
        STANDING_IMAGE_ID,
        FLIP,
        COLOR_MULT,
        POS_X,
        POS_Y,
        BGM,
        SFX,
        BG_IMG,
    }

    public void ParseCsvSheet(string csv)
    {
        // 1. 장면 ID 찾기
        // 2. 그 행의 장면데이터 입력
        // 3. 다음 행부터 ID열이 비어있다면 스탠딩이미지 데이터로 취급해 추가
        // 4. ID열이 채워져 있다면 장면 데이터로 보고 다음 장면 데이터 입력으로 이동


    }
#endif
}
