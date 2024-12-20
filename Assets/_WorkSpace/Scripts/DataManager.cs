using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Firebase.Extensions;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

public interface ICsvRowParseable
{
#if UNITY_EDITOR
    public void ParseCsvRow(string[] cells);
#endif
}

public interface ICsvSheetParseable
{
#if UNITY_EDITOR
    public void ParseCsvSheet(string csv);
#endif
}


public class DataManager : SingletonScriptable<DataManager>
{
    [SerializeField] List<CharacterData> characterDataList;
    private Dictionary<int, CharacterData> characterDataIdDic; // id 기반 검색용

    [SerializeField] List<StoryDirectingData> storyDirectingDataList;

    public UnityEvent onLoadUserDataCompleted;

    public CharacterData GetCharacterData(int id)
    {
        if (false == characterDataIdDic.ContainsKey(id))
        {
            Debug.LogWarning($"존재하지 않는 캐릭터 ID({id})가 요청됨");
            return null;
        }

        return characterDataIdDic[id];
    }

    private void OnEnable() => IndexData();

    /// <summary>
    /// 색인 생성
    /// </summary>
    private void IndexData()
    {
        characterDataIdDic = characterDataList.ToDictionary(item => item.Id);
    }

#if UNITY_EDITOR
    public const string SoundsAssetFolder = "Assets/Imports/Sounds";
    public const string PrefabsAssetFolder = "Assets/_WorkSpace/Prefabs";
    public const string SpritesAssetFolder = "Assets/_WorkSpace/Sprites";

    [SerializeField] string documentID;

    [SerializeField] string characterSheetId;
    [SerializeField] Object characterDataFolder;

    [ContextMenu("시트에서 캐릭터 데이터 불러오기")]
    private void GetCharacterDataFromSheet()
    {
        GetRowDataFromSheet<CharacterData>("0", characterDataFolder, characterDataList);
        IndexData();
    }

    [SerializeField] Object storyDirectingDataFolder;

    [ContextMenu("스토리 데이터 불러오기 테스트")]
    private void GetStroyDataTest()
    {
        storyDirectingDataList.Clear();
        GetSheetDataFromSheet<StoryDirectingData>("1890934115", storyDirectingDataFolder, storyDirectingDataList);
    }


    private void GetRowDataFromSheet<T>(string sheetId, Object dataFolder, List<T> dataList) where T : ScriptableObject, ICsvRowParseable
    {
        GoogleSheet.GetSheetData(documentID, sheetId, this, (succeed, result) =>
        {
            if (succeed == true)
            {
                dataList.Clear();
                string soFolderPath = AssetDatabase.GetAssetPath(dataFolder);

                string[] lines = result.Split("\r\n");
                foreach (string line in lines)
                {
                    string[] cells = line.Split(',');

                    // 0번열은 ID
                    // ID가 정수가 아니라면 데이터행이 아닌 것으로 간주(주석 등)
                    if (false == int.TryParse(cells[0], out int id))
                        continue;

                    // 1번열은 파일명
                    string soPath = $"{soFolderPath}/{cells[1]}.asset";

                    T soAsset;
                    if (System.IO.File.Exists(soPath))
                    {
                        soAsset = AssetDatabase.LoadAssetAtPath(soPath, typeof(T)) as T;
                        soAsset.ParseCsvRow(cells);

                        EditorUtility.SetDirty(soAsset);
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        soAsset = ScriptableObject.CreateInstance<T>();

                        soAsset.ParseCsvRow(cells);

                        AssetDatabase.CreateAsset(soAsset, soPath);
                    }

                    // 매니저에서 관리하기 위한 목록에 추가
                    dataList.Add(soAsset);
                }

                AssetDatabase.Refresh();

            }
            else
            {
                Debug.LogWarning("<color=red>읽기 실패!</color>");
            }

        });
    }

    private void GetSheetDataFromSheet<T>(string sheetId, Object dataFolder, List<T> dataList) where T : ScriptableObject, ICsvSheetParseable
    {
        GoogleSheet.GetSheetData(documentID, sheetId, this, (succeed, result) =>
        {
            if (succeed == true)
            {
                string soFolderPath = AssetDatabase.GetAssetPath(dataFolder);

                // 1A 셀에는 파일명 겸 식별자 넣을것
                string soPath = $"{soFolderPath}/{result.Substring(0, result.IndexOf(','))}.asset";

                T soAsset;
                if (System.IO.File.Exists(soPath))
                {
                    soAsset = AssetDatabase.LoadAssetAtPath(soPath, typeof(T)) as T;
                    soAsset.ParseCsvSheet(result);

                    EditorUtility.SetDirty(soAsset);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    soAsset = ScriptableObject.CreateInstance<T>();

                    soAsset.ParseCsvSheet(result);

                    AssetDatabase.CreateAsset(soAsset, soPath);
                }

                AssetDatabase.Refresh();

                dataList.Add(soAsset);
            }
            else
            {
                Debug.LogWarning("<color=red>읽기 실패!</color>");
            }

        });
    }
#endif

    [ContextMenu("유저데이터 테스트")]
    public void LoadUserData()
    {
        if (Application.isPlaying == false)
        {
            Debug.LogWarning("플레이모드가 아닐 경우 오작동할 수 있습니다");
        }

        BackendManager.Instance.UseDummyUserDataRef(0); // 테스트코드

        BackendManager.CurrentUserDataRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"접속 실패!!");
                return;
            }

            Dictionary<string, object> userData = task.Result.Value as Dictionary<string, object>;

            // 캐릭터 데이터가 존재한다면
            if (userData.ContainsKey("Characters"))
            {
                Dictionary<string, object> charactersData = userData["Characters"] as Dictionary<string, object>;

                // DB의 데이터(레벨)값을 캐싱
                foreach (KeyValuePair<string, object> dataPair in charactersData)
                {
                    // DB에서 가져온 키값 문자열 int로 파싱하기 vs 문자열을 키값으로 쓰기
                    if (false == int.TryParse(dataPair.Key, out int id))
                    {
                        Debug.LogWarning($"잘못된 키 값({dataPair.Key})");
                        continue;
                    }

                    characterDataIdDic[id].Level.Value = ((int)(long)dataPair.Value);
                }
            }

            onLoadUserDataCompleted?.Invoke();
        });

    }
}
