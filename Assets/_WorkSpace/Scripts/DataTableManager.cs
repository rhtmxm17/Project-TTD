using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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


public class DataTableManager : SingletonScriptable<DataTableManager>
{
    [SerializeField] List<CharacterData> characterDataList;
    private Dictionary<int, CharacterData> characterDataIdDic; // id 기반 검색용

    [SerializeField] List<ItemData> itemDataList;
    private Dictionary<int, ItemData> itemDataIdDic; // id 기반 검색용

    [SerializeField] List<StoryDirectingData> storyDirectingDataList;


    public CharacterData GetCharacterData(int id)
    {
        if (false == characterDataIdDic.ContainsKey(id))
        {
            Debug.LogWarning($"존재하지 않는 캐릭터 ID({id})가 요청됨");
            return null;
        }

        return characterDataIdDic[id];
    }

    public ItemData GetItemData(int id)
    {
        if (false == itemDataIdDic.ContainsKey(id))
        {
            Debug.LogWarning($"존재하지 않는 아이템 ID({id})가 요청됨");
            return null;
        }

        return itemDataIdDic[id];
    }

    private void OnEnable() => IndexData();

    /// <summary>
    /// 색인 생성
    /// </summary>
    private void IndexData()
    {
        characterDataIdDic = characterDataList.ToDictionary(item => item.Id);
        itemDataIdDic = itemDataList.ToDictionary(item => item.Id);
    }

#if UNITY_EDITOR
    public const string SoundsAssetFolder = "Assets/Imports/Sounds";
    public const string PrefabsAssetFolder = "Assets/_WorkSpace/Prefabs";
    public const string SpritesAssetFolder = "Assets/_WorkSpace/Sprites";
    public const string SkillAssetFolder = "Assets/_WorkSpace/Datas/Skills";

    [SerializeField] string documentID;

    [SerializeField] Object characterDataFolder;
    [SerializeField] Object itemDataFolder;
    [SerializeField] Object storyDirectingDataFolder;

    private string characterSheetId = "0";
    private string itemSheetId = "1467425655";

    [ContextMenu("시트에서 캐릭터 데이터 불러오기")]
    private void GetCharacterDataFromSheet()
    {
        GetRowDataFromSheet<CharacterData>(characterSheetId, characterDataFolder, characterDataList);
        IndexData();
    }

    [ContextMenu("시트에서 아이템 데이터 불러오기")]
    private void GetItemDataFromSheet()
    {
        GetRowDataFromSheet<ItemData>(itemSheetId, itemDataFolder, itemDataList);
        IndexData();
    }

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

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
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
                }
                else
                {
                    soAsset = ScriptableObject.CreateInstance<T>();

                    soAsset.ParseCsvSheet(result);

                    AssetDatabase.CreateAsset(soAsset, soPath);
                }

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
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

}
