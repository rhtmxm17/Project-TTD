using System;
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine.Networking;


public static class GoogleSheet
{

    /// <summary>
    /// 구글 엑셀 시트를 받아오는 에디터 코루틴 메서드
    /// </summary>
    /// <param name="documentID">공유 시트 id</param>
    /// <param name="sheetID">시트 페이지 (g)id</param>
    /// <param name="onwer">코루틴이 돌아갈 behavior</param>
    /// <param name="process">다운로드 완료시 실행할 함수</param>
    public static void GetSheetData(string documentID, string sheetID, object onwer, Action<bool, string> process = null)
    {
#if UNITY_EDITOR
        EditorCoroutineUtility.StartCoroutine(GetSheetDataCo(documentID, sheetID, process), onwer);
#endif
    }

    /// <summary>
    /// 구글 엑셀시트에서 csv값을 비동기적으로 읽어옴
    /// </summary>
    /// <param name="documentID">공유 시트 id</param>
    /// <param name="sheetID">시트 페이지 (g)id</param>
    /// <param name="process">다운로드 완료시 실행할 함수</param>
    /// <returns></returns>
    private static IEnumerator GetSheetDataCo(string documentID, string sheetID, Action<bool, string> process = null)
    {

        string url = $"https://docs.google.com/spreadsheets/d/{documentID}/export?format=tsv&gid={sheetID}";

        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode != 200)
        {

            process?.Invoke(false, null);
            yield break;

        }

        process?.Invoke(true, req.downloadHandler.text);

    }

}
