#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class SearchAsset
{
    /*
     디버그용으로 인스턴스 생성되게 함
     */
    private static SearchAsset s_Instance;

    private static SearchAsset instance
    {
        get
        {
            if (s_Instance == null)
            {
                SearchService.Refresh();
                s_Instance = new SearchAsset();
            }

            return s_Instance;
        }
    }
    private SearchAsset() { }

    private const string SoundsDirQuery = "dir:\"Assets/Imports/Sounds\"";
    private const string SpritesDirQuery = "(dir:\"Assets/_WorkSpace/Sprites\" or dir:\"Assets/Imports/Etcetera\")";
    private const string PrefabsDirQuery = "dir:\"Assets/_WorkSpace/Prefabs\"";
    private const string SODirQuery = "dir:\"Assets/_WorkSpace/Datas\"";

    [MenuItem("Tools/에셋 색인 생성")]
    private static void CreateIndex()
    {
        SearchService.CreateIndex("SearchDB",
            IndexingOptions.Properties | IndexingOptions.Dependencies |
            IndexingOptions.Types | IndexingOptions.Keep,
            roots: new string[] { "Assets/_WorkSpace", "Assets/Imports/Sounds", "Assets/Imports/Etcetera" }, // 루트 경로
            includes: null, // 포함 문자열
            excludes: new string[] { ".cs" }, // 제외 문자열
            (name, path, finished) =>
            {
                Debug.Log($"asset index {name} created at {path}");
                finished();
            });
    }


    /// <summary>
    /// 미리 지정된 경로 내에서 사운드 애셋을 찾습니다<br/>
    /// 실패시 null 반환
    /// </summary>
    /// <param name="assetName">검색할 사운드 애셋 파일명</param>
    /// <returns>검색된 애셋</returns>
    public static AudioClip SearchAudioClipAsset(string assetName) => instance.SearchAudioClipAsset_(assetName);

    private AudioClip SearchAudioClipAsset_(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
            return null;

        using var searchContext = SearchService.CreateContext($"p: {SoundsDirQuery} t:AudioClip name={assetName}");
        // Initiate the query and get the results.
        // Note: it is recommended to use SearchService.Request if you wish to fetch the items asynchronously.
        var results = SearchService.GetItems(searchContext, SearchFlags.WantsMore | SearchFlags.Synchronous);

        if (results.Count == 0)
        {
            Debug.LogWarning($"AudioClip:{assetName}을 찾지 못함");
            return null;
        }

        if (results.Count != 1)
        {
            Debug.LogWarning($"AudioClip:{assetName}이 지정된 경로 내에 복수 존재함");
        }

        return results[0].ToObject<AudioClip>();
    }

    /// <summary>
    /// 미리 지정된 경로 내에서 스프라이트 애셋을 찾습니다<br/>
    /// 실패시 null 반환
    /// </summary>
    /// <param name="assetName">검색할 스프라이트 애셋 파일명</param>
    /// <returns>검색된 애셋</returns>
    public static Sprite SearchSpriteAsset(string assetName) => instance.SearchSpriteAsset_(assetName);

    private Sprite SearchSpriteAsset_(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
            return null;

        using var searchContext = SearchService.CreateContext($"p: {SpritesDirQuery} t:Sprite name={assetName}");
        // Initiate the query and get the results.
        // Note: it is recommended to use SearchService.Request if you wish to fetch the items asynchronously.
        var results = SearchService.GetItems(searchContext, SearchFlags.WantsMore | SearchFlags.Synchronous);

        if (results.Count == 0)
        {
            Debug.LogWarning($"Sprite:{assetName}을 찾지 못함");
            return null;
        }

        if (results.Count != 1)
        {
            Debug.LogWarning($"Sprite:{assetName}이 지정된 경로 내에 복수 존재함");
        }

        return results[0].ToObject<Sprite>();
    }

    /// <summary>
    /// 미리 지정된 경로 내에서 프리펩을 찾습니다<br/>
    /// 실패시 null 반환
    /// </summary>
    /// <typeparam name="T">찾고자 하는 프리펩의 MonoBehaviour 타입</typeparam>
    /// <param name="assetName">검색할 애셋 파일명</param>
    /// <returns>검색된 애셋</returns>
    public static T SearchPrefabAsset<T>(string assetName) where T : MonoBehaviour => instance.SearchPrefabAsset_<T>(assetName);

    private T SearchPrefabAsset_<T>(string assetName) where T : MonoBehaviour
    {
        if (string.IsNullOrEmpty(assetName))
            return null;

        using var searchContext = SearchService.CreateContext($"p: {PrefabsDirQuery} t:{typeof(T).Name} name={assetName}.prefab");

        // 생성된 쿼리로 검색
        var results = SearchService.GetItems(searchContext, SearchFlags.WantsMore | SearchFlags.Synchronous);

        if (results.Count == 0)
        {
            Debug.LogWarning($"{typeof(T).Name}:{assetName}을 찾지 못함");
            return null;
        }

        if (results.Count != 1)
        {
            Debug.LogWarning($"{typeof(T).Name}:{assetName}이 지정된 경로 내에 복수 존재함");
        }

        return results[0].ToObject<T>();
    }

    /// <summary>
    /// 미리 지정된 경로 내에서 프리펩을 찾습니다<br/>
    /// 실패시 null 반환
    /// </summary>
    /// <param name="assetName">검색할 애셋 파일명</param>
    /// <returns>검색된 애셋</returns>
    public static GameObject SearchPrefabAsset(string assetName) => instance.SearchPrefabAsset_(assetName);

    public GameObject SearchPrefabAsset_(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
            return null;

        using var searchContext = SearchService.CreateContext($"p: {PrefabsDirQuery} t:prefab name={assetName}.prefab");

        // 생성된 쿼리로 검색
        var results = SearchService.GetItems(searchContext, SearchFlags.WantsMore | SearchFlags.Synchronous);

        if (results.Count == 0)
        {
            Debug.LogWarning($"prefab:{assetName}을 찾지 못함");
            return null;
        }

        if (results.Count != 1)
        {
            Debug.LogWarning($"prefab:{assetName}이 지정된 경로 내에 복수 존재함");
        }

        return results[0].ToObject<GameObject>();
    }

    /// <summary>
    /// 미리 지정된 경로 내에서 SO를 찾습니다<br/>
    /// 실패시 null 반환
    /// </summary>
    /// <typeparam name="T">찾고자 하는 SO의 타입</typeparam>
    /// <param name="assetName">검색할 애셋 파일명</param>
    /// <returns>검색된 애셋</returns>
    public static T SearchSOAsset<T>(string assetName) where T : ScriptableObject => instance.SearchSOAsset_<T>(assetName);

    public T SearchSOAsset_<T>(string assetName) where T : ScriptableObject
    {
        if (string.IsNullOrEmpty(assetName))
            return null;

        using var searchContext = SearchService.CreateContext($"p: {SODirQuery} t:{typeof(T).Name} name={assetName}.asset");

        // 생성된 쿼리로 검색
        var results = SearchService.GetItems(searchContext, SearchFlags.NoIndexing | SearchFlags.WantsMore | SearchFlags.Synchronous);

        if (results.Count == 0)
        {
            Debug.LogWarning($"{typeof(T).Name}:{assetName}을 찾지 못함");
            return null;
        }

        if (results.Count != 1)
        {
            Debug.LogWarning($"{typeof(T).Name}:{assetName}이 지정된 경로 내에 복수 존재함");
        }

        return results[0].ToObject<T>();
    }
}
#endif // UNITY_EDITOR