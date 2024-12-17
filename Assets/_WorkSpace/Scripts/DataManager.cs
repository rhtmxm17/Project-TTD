using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface ISheetManageable
{
#if UNITY_EDITOR
    public void ParseCsvLine(string[] cells);
#endif
}

public class DataManager : SingletonScriptable<DataManager>
{
    [SerializeField] List<CharacterData> characterDataList;
    private Dictionary<int, CharacterData> characterDataIdDic; // id 기반 검색용

    public CharacterData GetCharacterData(int id)
    {
        if (false == characterDataIdDic.ContainsKey(id))
        {
            Debug.LogWarning($"존재하지 않는 캐릭터 ID({id})가 요청됨");
            return null;
        }

        return characterDataIdDic[id];
    }

#if UNITY_EDITOR
    [SerializeField] string documentID;

    [SerializeField] string characterSheetId;
    [SerializeField] Object characterDataFolder;

    [ContextMenu("시트에서 캐릭터 데이터 불러오기")]
    private void GetCharacterDataFromSheet()
    {
        GetDataFromSheet<CharacterData>("0", characterDataFolder, characterDataList);

        // 색인 생성
        characterDataIdDic = characterDataList.ToDictionary(item => item.Id);
    }

    private void GetDataFromSheet<T>(string sheetId, Object dataFolder, List<T> dataList) where T : ScriptableObject, ISheetManageable
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
                        soAsset.ParseCsvLine(cells);

                        EditorUtility.SetDirty(soAsset);
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        soAsset = ScriptableObject.CreateInstance<T>();

                        soAsset.ParseCsvLine(cells);

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
#endif

    public void LoadUserData()
    {
        // TODO: DB에서 유저 데이터 긁어서 SO에 넣기(ex: 캐릭터 레벨)
    }
}
