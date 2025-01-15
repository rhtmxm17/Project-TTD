using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CSVTester : MonoBehaviour
{
    [SerializeField]
    string path;
    [SerializeField]
    string gid;

    /// <summary>
    /// 에디터에서 실행, 시트 데이터를 읽어와 저장.
    /// 저장 경로는 추후 수정
    /// </summary>
    [ContextMenu("test")]
    public void GetData()
    {

#if UNITY_EDITOR

        GoogleSheet.GetSheetData(path, gid, this, (b, s) =>
        {

            string path = "Assets/CsvPaser/soTestSO.asset";

            if (b == true)
            {

                
                if (File.Exists(path))
                {
                    var so = AssetDatabase.LoadAssetAtPath(path, typeof(CSVTestSO)) as CSVTestSO;

                    so.text = s;

                    EditorUtility.SetDirty(so);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                }
                else
                {
                    var so = ScriptableObject.CreateInstance<CSVTestSO>();

                    so.text = s;

                    AssetDatabase.CreateAsset(so, path);
                }
            }
            else
            {
                Debug.Log("<color=red>읽기 실패!</color>");
            }

        });

#endif

    }
}
