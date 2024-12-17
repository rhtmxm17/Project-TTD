using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField]
    string documentID;
    [SerializeField]
    string sheetID;
    [SerializeField]
    Object folder;

    [ContextMenu("시트에서 캐릭터 데이터 불러오기")]
    private void GetCharacterDataFromSheet()
    {
        GoogleSheet.GetSheetData(documentID, sheetID, this, (succeed, result) =>
        {
            if (succeed == true)
            {
                string soFolderPath = AssetDatabase.GetAssetPath(folder);

                string[] lines = result.Split("\n");
                foreach (string line in lines)
                {
                    string[] cells = line.Split(',');

                    if (false == int.TryParse(cells[0], out int id))
                    {
                        Debug.Log("데이터 행이 아님");
                        continue;
                    }

                    string soPath = $"{soFolderPath}/{cells[1]}.asset";

                    if (System.IO.File.Exists(soPath))
                    {
                        var so = AssetDatabase.LoadAssetAtPath(soPath, typeof(CharacterData)) as CharacterData;

                        AssetDatabase.GetAssetPath(folder);

                        so.ParseCsvLine(cells);

                        EditorUtility.SetDirty(so);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                    }
                    else
                    {
                        var so = ScriptableObject.CreateInstance<CharacterData>();

                        so.ParseCsvLine(cells);

                        AssetDatabase.CreateAsset(so, soPath);
                    }
                }
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
