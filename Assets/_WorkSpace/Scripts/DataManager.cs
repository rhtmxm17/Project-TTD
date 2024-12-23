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
                Dictionary<string, object> allCharacterData = userData["Characters"] as Dictionary<string, object>;

                // DB의 데이터를 캐싱
                foreach (KeyValuePair<string, object> dataPair in allCharacterData)
                {
                    // DB에서 가져온 키값 문자열 int로 파싱하기 vs 문자열을 키값으로 쓰기
                    if (false == int.TryParse(dataPair.Key, out int id))
                    {
                        Debug.LogWarning($"잘못된 키 값({dataPair.Key})");
                        continue;
                    }

                    Dictionary<string, object> characterData = dataPair.Value as Dictionary<string, object>;

                    if (characterData.ContainsKey("Level"))
                    {
                        characterDataIdDic[id].Level.SetValueOnLoading((int)(long)characterData["Level"]);
                    }
                    if (characterData.ContainsKey("Enhancement"))
                    {
                        characterDataIdDic[id].Enhancement.SetValueOnLoading((int)(long)characterData["Enhancement"]);
                    }
                }
            }

            onLoadUserDataCompleted?.Invoke();
        });

    }

    #region DB 데이터 갱신
    public UpdateDbChain StartUpdateStream()
    {
        return new UpdateDbChain();
    }

    /// <summary>
    /// 실제로 DB의 유저 대이터 갱신을 담당하는 클래스
    /// </summary>
    public class UpdateDbChain
    {
        private Dictionary<string, object> updates = new Dictionary<string, object>();
        private event UnityAction propertyCallbackOnSubmit;

        public class PropertyAdapter<T> where T : System.IEquatable<T>
        {
            public T Value { get; private set; }

            public event UnityAction<T> onValueChanged;

            public string Key { get; private set; }

            public PropertyAdapter(string key)
            {
                this.Key = key;
            }

            /// <summary>
            /// 로딩 단계에서 초기값 입력을 위한 메서드
            /// </summary>
            /// <param name="value"></param>
            public void SetValueOnLoading(T value) // DataManager 바깥에선 숨기는 방법이 없을까?
            {
                this.Value = value;
            }

            /// <summary>
            /// UpdateDbChain.SetDBValue()에서 갱신 대상 등록을 위한 메서드<br/>
            /// 다른 용도의 사용을 상정하지 않음
            /// </summary>
            /// <param name="updateDbChain"></param>
            /// <param name="value"></param>
            public void RegisterToChain(UpdateDbChain updateDbChain, T value) // UpdateDbChain 바깥에선 숨기는 방법이 없을까?
            {
                updateDbChain.updates[Key] = value;
                updateDbChain.propertyCallbackOnSubmit += () =>
                {
                    Value = value;
                    onValueChanged?.Invoke(value);
                };
            }
        }

        public UpdateDbChain SetDBValue<T>(PropertyAdapter<T> property, T value) where T : System.IEquatable<T>
        {
            // 값이 갱신되지 않았다면 등록하지 않음
            if (property.Value.Equals(value))
                return this;

#if DEBUG
            if (updates.ContainsKey(property.Key))
            {
                Debug.LogWarning("한 스트림에 데이터를 두번 갱신하고 있음");
            }
#endif //DEBUG
            property.RegisterToChain(this, value);

            return this;
        }

        /// <summary>
        /// (비동기)UpdateDbChain.SetDBValue로 등록된 갱신사항 목록으로 DB에 갱신 요청을 전송한다
        /// </summary>
        /// <param name="onCompleteCallback">작업 완료시 결과를 반환받을 callback (성공시 true)</param>
        public void Submit(UnityAction<bool> onCompleteCallback)
        {
            // 갱신사항이 없다면 즉시 완료 처리
            if (updates.Count == 0)
            {
                onCompleteCallback?.Invoke(true);
                return;
            }

            BackendManager.CurrentUserDataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"요청 실패함");
                    onCompleteCallback?.Invoke(false);
                    return;
                }

                Debug.Log($"데이터 갱신 요청 성공");
                propertyCallbackOnSubmit?.Invoke();
                onCompleteCallback?.Invoke(true);
            });
        }
    }
    #endregion DB 데이터 갱신
}


/// <summary>
/// DB에서 관리되는 유저 데이터
/// </summary>
/// <typeparam name="T">string, long, double, bool</typeparam>
public class UserDataProperty<T> : DataManager.UpdateDbChain.PropertyAdapter<T> where T : System.IEquatable<T>
{
    public UserDataProperty(string key) : base(key) { }
}