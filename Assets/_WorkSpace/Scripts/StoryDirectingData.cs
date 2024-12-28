using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(StoryDirectingData))]
public class StoryDirectingDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("테이블 매니저 바로가기"))
        {
            UnityEditor.Selection.activeObject = DataTableManager.Instance;
        }

        base.OnInspectorGUI();
    }
}
#endif

public class StoryDirectingData : ScriptableObject, ICsvSheetParseable
{
    [SerializeField] int id;
    public int Id => id;

    [SerializeField] string title;
    public string Title => title;

    [System.Serializable]
    public struct Dialogue
    {
        public string Speaker;
        [TextArea] public string Script;

        // 해당 다이얼로그 재생과 함께 진행되는 갱신사항들
        // null이라면 해당 내용은 갱신하지 않음을 의미
        public AudioClip Bgm;
        public AudioClip Sfx;
        public Sprite BackgroundSprite;
        public List<TransitionInfo> Transitions; // 스탠딩 이미지 갱신사항
    }

    [System.Serializable]
    public struct TransitionInfo
    {
        public int StandingImageId; // 어느 이미지를 갱신할 것인지
        public bool Active; // 출현 또는 숨기기
        public bool Flip;
        public float ColorMultiply; // 1(일반)~0(어두움), rgb에 각각 곱할 값
        public Vector2 Position; // 유효 영역 기준, 좌하단을 (0, 0) 우상단을 (1, 1)로 하는 위치 좌표
    }

    [System.Serializable]
    public struct StandingImage
    {
        public int ActorId; // 해당 스토리에서 사용되는 ID(직렬화된 배열 인덱스)
        public Sprite ImageSprite;
    }

    [SerializeField] Dialogue[] dialogues;
    public Dialogue[] Dialogues => dialogues;

    [SerializeField] StandingImage[] standingImages;
    public StandingImage[] StandingImages => standingImages;

#if UNITY_EDITOR
    private enum Column
    {
        ID,         // 다이얼로그
        SPEAKER,    // 다이얼로그
        SCRIPT,     // 다이얼로그
        STANDING_IMAGE_ID,  // 스탠딩 이미지
        LEAVE,              // 스탠딩 이미지
        FLIP,               // 스탠딩 이미지
        COLOR_MULT,         // 스탠딩 이미지
        POS_X,              // 스탠딩 이미지
        POS_Y,              // 스탠딩 이미지
        TRANSITION,         // 스탠딩 이미지
        BGM,        // 다이얼로그
        SFX,        // 다이얼로그
        BG_IMG,     // 다이얼로그
    }

    public void ParseCsvSheet(int sheetId, string title, string csv)
    {
        this.id = sheetId;
        this.title = title;

        // 1. 장면 ID 찾기
        // 2. 그 행의 장면데이터 입력
        // 3. 다음 행부터 ID열이 비어있다면 스탠딩이미지 데이터로 취급해 추가
        // 4. ID열이 채워져 있다면 장면 데이터로 보고 다음 장면 데이터 입력으로 이동

        Dictionary<string, StandingImage> tempStandingImages = new Dictionary<string, StandingImage>();
        List<Dialogue> tempDialogues = new List<Dialogue>();

        string[] lines = csv.Split("\r\n");
        foreach (string line in lines)
        {
            string[] cells = line.Split(',');

            // 0번열은 다이얼로그 ID
            if (int.TryParse(cells[0], out int _))
            {
                Dialogue parsed = new Dialogue();
                parsed.Transitions = new List<TransitionInfo>();

                // SPEAKER
                parsed.Speaker = cells[(int)Column.SPEAKER];

                // SCRIPT
                parsed.Script = cells[(int)Column.SCRIPT]
                    .Trim('\"') // 줄바꿈 등 적용시 자동으로 앞뒤에 씌워짐
                    .Trim('|') // 큰 따옴표 씌우기용
                    .Replace("\"\"", "\""); // 큰 따옴표가 2개씩 들어오는거 해결

                // BGM
                if (false == string.IsNullOrEmpty(cells[(int)Column.BGM]))
                {
                    parsed.Bgm = AssetDatabase.LoadAssetAtPath<AudioClip>($"{DataTableManager.SoundsAssetFolder}/{cells[(int)Column.BGM]}");
                }

                // SFX
                if (false == string.IsNullOrEmpty(cells[(int)Column.SFX]))
                {
                    parsed.Sfx = AssetDatabase.LoadAssetAtPath<AudioClip>($"{DataTableManager.SoundsAssetFolder}/{cells[(int)Column.SFX]}");
                }

                // BG_IMG
                if (false == string.IsNullOrEmpty(cells[(int)Column.BG_IMG]))
                {
                    parsed.BackgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{DataTableManager.SpritesAssetFolder}/{cells[(int)Column.BG_IMG]}.asset");
                }

                tempDialogues.Add(parsed);
            }
            // ID가 정수가 아니라면 다이얼로그행이 아님(주석 또는 트랜지션)
            else
            {
                // 다이얼로그가 비어있다면 주석행
                if (tempDialogues.Count == 0)
                    continue;

                TransitionInfo parsed = new TransitionInfo();

                // STANDING_IMAGE_ID: 등장인물 번호 조회 또는 신규 등장인물 가져오기
                string imageKey = cells[(int)Column.STANDING_IMAGE_ID];
                if (tempStandingImages.ContainsKey(imageKey))
                {
                    parsed.StandingImageId = tempStandingImages[imageKey].ActorId;
                }
                else
                {
                    parsed.StandingImageId = tempStandingImages.Count;

                    StandingImage loaded = new StandingImage();
                    loaded.ActorId = tempStandingImages.Count;

                    Debug.LogWarning("임시 코드 사용중: 이미지 식별자에서 이미지 파일명 가져오는 기능으로 변경 필요");
                    loaded.ImageSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{DataTableManager.SpritesAssetFolder}/{imageKey}.asset");

                    tempStandingImages.Add(imageKey, loaded);
                }

                // LEAVE: 테이블값이 T면 해당 캐릭터 퇴장
                parsed.Active = ("T" != cells[(int)Column.LEAVE]);

                // FLIP: 테이블값이 T면 해당 캐릭터는 좌우 반전 상태
                parsed.Flip = ("T" == cells[(int)Column.FLIP]);

                // COLOR_MULT
                if (false == float.TryParse(cells[(int)Column.COLOR_MULT], out parsed.ColorMultiply))
                {
                    Debug.LogWarning($"잘못된 자료형이 입력됨(요구사항:float, 입력된 데이터:{cells[(int)Column.COLOR_MULT]}");
                    continue;
                }

                // POS_X
                if (false == float.TryParse(cells[(int)Column.POS_X], out parsed.Position.x))
                {
                    Debug.LogWarning($"잘못된 자료형이 입력됨(요구사항:float, 입력된 데이터:{cells[(int)Column.POS_X]}");
                    continue;
                }

                // POS_Y
                if (false == float.TryParse(cells[(int)Column.POS_Y], out parsed.Position.y))
                {
                    Debug.LogWarning($"잘못된 자료형이 입력됨(요구사항:float, 입력된 데이터:{cells[(int)Column.POS_Y]}");
                    continue;
                }

                // 0~10 범위를 0~1 범위로 변환
                parsed.Position *= 0.1f;

                // 파싱된 연출 정보를 현재 대사에 등록
                tempDialogues[tempDialogues.Count - 1].Transitions.Add(parsed);
            }
        }

        // 임시 데이터를 직렬화 필드에 저장
        standingImages = new StandingImage[tempStandingImages.Count];
        foreach (var pair in tempStandingImages)
        {
            standingImages[pair.Value.ActorId] = pair.Value;
        }

        dialogues = tempDialogues.ToArray();
    }
#endif
}
