using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase.Database;
using System;
using System.Collections.ObjectModel;

public class UserDataManager : SingletonBehaviour<UserDataManager>
{
    /// <summary>
    /// 24. 12. 27 김민태 캐릭터 소유 목록 추가
    /// </summary>

    #region 소유 캐릭터 테스트

    [NonSerialized]
    List<int> haveCharacterIdxList = new List<int>();
    public ReadOnlyCollection<int> HaveCharacterIdxList => haveCharacterIdxList.AsReadOnly();

    /// <summary>
    /// 캐릭터를 가지고있는지 여부 반환
    /// </summary>
    /// <param name="characterIdx">체크할 캐릭터 ID</param>
    /// <returns></returns>
    public bool HasCharacter(int characterIdx)
    { 
        return haveCharacterIdxList.Contains(characterIdx);
        
        // return (GetHasCharacterData(characterIdx).Value != 0);
    }

    public void ApplyCharacter(int characterIdx)
    {
        //실제 데이터 갱신.
        CharacterData chData = GameManager.TableData.GetCharacterData(characterIdx);

        StartUpdateStream()
            .SetDBValue(chData.Has, 1)
            .SetDBValue(chData.Level, 1)
            .SetDBValue(chData.Enhancement, 0)
            .Submit((result) =>
            {
                Debug.Log($"캐릭터 확득 결과:{result}");

                haveCharacterIdxList.Add(characterIdx);
            });
    }

    #endregion

    public UnityEvent onLoadUserDataCompleted { get; private set; } = new UnityEvent();

    public UserProfile Profile { get; private set; } = new UserProfile();

    public GamePlayData PlayData { get; private set; } = new GamePlayData();

    #region SO 연계형 유저 데이터
    

    private Dictionary<int, UserDataInt> itemNumberDict = new Dictionary<int, UserDataInt>();
    public UserDataInt GetItemNumber(int id)
    {
        if (itemNumberDict.ContainsKey(id))
            return itemNumberDict[id];
        else
        {
            UserDataInt udInt = new UserDataInt($"Items/{id}");
            itemNumberDict.Add(id, udInt);
            // 아이템은 SO의 이벤트를 발생시킴
            udInt.onValueChanged += GameManager.TableData.GetItemData(id).InvokeNumberChanged;
            return udInt;
        }
    }

    private Dictionary<int, UserDataInt> stageClearCountDict = new Dictionary<int, UserDataInt>();
    public UserDataInt GetStageClearCount(int id)
    {
        if (stageClearCountDict.ContainsKey(id))
            return stageClearCountDict[id];
        else
        {
            UserDataInt udInt = new UserDataInt($"Stages/{id}/ClearCount");
            stageClearCountDict.Add(id, udInt);
            return udInt;
        }
    }

    private Dictionary<int, UserDataInt> hasCharacterDict = new Dictionary<int, UserDataInt>();
    public UserDataInt GetHasCharacterData(int id)
    {
        if (hasCharacterDict.ContainsKey(id))
            return hasCharacterDict[id];
        else
        {
            UserDataInt udInt = new UserDataInt($"Characters/{id}/Has");
            hasCharacterDict.Add(id, udInt);
            return udInt;
        }
    }

    private Dictionary<int, UserDataInt> characterLevelDict = new Dictionary<int, UserDataInt>();
    public UserDataInt GetCharacterLevel(int id)
    {
        if (characterLevelDict.ContainsKey(id))
            return characterLevelDict[id];
        else
        {
            UserDataInt udInt = new UserDataInt($"Characters/{id}/Level", 1);
            characterLevelDict.Add(id, udInt);
            return udInt;
        }
    }

    private Dictionary<int, UserDataInt> characterEnhancementDict = new Dictionary<int, UserDataInt>();
    public UserDataInt GetCharacterEnhancement(int id)
    {
        if (characterEnhancementDict.ContainsKey(id))
            return characterEnhancementDict[id];
        else
        {
            UserDataInt udInt = new UserDataInt($"Characters/{id}/Enhancement");
            characterEnhancementDict.Add(id, udInt);
            return udInt;
        }
    }

    private Dictionary<int, UserDataInt> characterMileageDict = new Dictionary<int, UserDataInt>();
    public UserDataInt GetCharacterMileage(int id)
    {
        if (characterMileageDict.ContainsKey(id))
            return characterMileageDict[id];
        else
        {
            UserDataInt udInt = new UserDataInt($"Characters/{id}/EnhanceMileagePerMill");
            characterMileageDict.Add(id, udInt);
            return udInt;
        }
    }

    private Dictionary<int, UserDataInt> packageBoughtDict = new Dictionary<int, UserDataInt>();
    public UserDataInt GetPackageBought(int id)
    {
        if (packageBoughtDict.ContainsKey(id))
            return packageBoughtDict[id];
        else
        {
            UserDataInt udInt = new UserDataInt($"ShopItems/{id}/Bought");
            packageBoughtDict.Add(id, udInt);
            return udInt;
        }
    }

    #endregion SO 연계형 유저 데이터

    public class UserProfile
    {
        public UserDataString Name { get; private set; } = new UserDataString($"Profile/Name", "이름 없음");
        public UserDataInt IconIndex { get; private set; } = new UserDataInt($"Profile/IconIndex");
        public UserDataInt Level { get; private set; } = new UserDataInt($"Profile/Level", 1);
        public UserDataString Introduction { get; private set; } = new UserDataString($"Profile/Introduction", "소개문 없음");
        public UserDataInt Rank { get; private set; } = new UserDataInt($"Profile/Rank", 999);


        public UserDataInt MyroomBgIdx { get; private set; } = new UserDataInt($"Profile/roomBG", 0);
        public UserDataInt MyroomCharaIdx { get; private set; } = new UserDataInt($"Profile/roomChara", 1);
    }

    public class GamePlayData
    {
        public const int MaxRoomIndex = 5;
        public const int MaxTutorialDone = 5;

        public UserDataDateTime EggGainTimestamp { get; private set; } = new UserDataDateTime("PlayData/EggGainTimestamp");
        public UserDataDateTime IdleRewardTimestamp { get; private set; } = new UserDataDateTime("PlayData/IdleRewardTimestamp");


        public UserDataDictionaryLong BatchInfo { get; private set; } = new UserDataDictionaryLong("PlayData/BatchInfo");
        public UserDataDictionaryLong GoldDungeonClearRate { get; private set; } = new UserDataDictionaryLong("PlayData/GoldDungeonClearRate");
        public UserDataDictionaryLong ExpDungeonClearRate { get; private set; } = new UserDataDictionaryLong("PlayData/ExpDungeonClearRate");
        public UserDataDictionaryLong EnforceDungeonClearRate { get; private set; } = new UserDataDictionaryLong("PlayData/EnforceDungeonClearRate");


        public List<UserDataInt> HasRoom = new List<UserDataInt>(MaxRoomIndex);

        /// <summary>
        /// 각 번호의 튜토리얼 완료 여부
        /// </summary>
        public List<UserDataInt> TutorialDone = new List<UserDataInt>(MaxRoomIndex);

        public GamePlayData()
        {
            for (int i = 0; i < MaxRoomIndex; i++)
            {
                HasRoom.Add(new UserDataInt($"PlayData/HasRoom/{i}"));
            }

            for (int i = 0; i < MaxRoomIndex; i++)
            {
                TutorialDone.Add(new UserDataInt($"PlayData/TutorialDone/{i}"));
            }
        }
    }

    private void Awake()
    {
        RegisterSingleton(this);
    }

    /// <summary>
    /// 테스트용 가인증 코드입니다
    /// </summary>
    /// <param name="DummyNumber">가인증 uid값 뒤쪽에 붙일 번호</param>
    /// <returns></returns>
    public void TryInitDummyUserAsync(int DummyNumber, UnityAction onCompletedCallback)
    {
        GameManager.Instance.StartCoroutine(InitDummyUser(DummyNumber, onCompletedCallback));
    }

    private IEnumerator InitDummyUser(int DummyNumber, UnityAction onCompletedCallback)
    {
        // Database 초기화 대기
        yield return new WaitWhile(() => GameManager.Database == null);
        // Auth 초기화 대기
        yield return new WaitWhile(() => GameManager.Auth == null);
        
        GameManager.Instance.StartShortLoadingUI();

        if (BackendManager.CurrentUserDataRef == null) // CurrentUserDataRef가 비어있다면 더미로 등록
        {
            BackendManager.Instance.UseDummyUserDataRef(DummyNumber); // 테스트코드
        }
        
        onLoadUserDataCompleted.AddListener(() =>
        {
            GameManager.Instance.StopShortLoadingUI();
            onCompletedCallback?.Invoke();
        });
        LoadUserData();
    }

    [ContextMenu("유저데이터 테스트")]
    public void LoadUserData()
    {
        if (Application.isPlaying == false)
        {
            Debug.LogWarning("플레이모드가 아닐 경우 오작동할 수 있습니다");
        }

        BackendManager.CurrentUserDataRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"접속 실패!!");
                return;
            }

            DataSnapshot userData = task.Result;

            if (userData.Value == null)
            {
                // 첫 접속일 경우 프로필 생성
                // TODO: UID 할당
                CharacterData startingChara = DataTableManager.Instance.GetCharacterData(1);
                ItemData startinCharaItem = DataTableManager.Instance.GetItemData(2001);

                StartUpdateStream()
                    .SetDBValue(this.Profile.Name, $"새로운 테스터{UnityEngine.Random.Range(1000, 10000)}")
                    .SetDBValue(this.Profile.Level, 1)
                    .SetDBValue(startingChara.Has, 1)
                    .SetDBValue(startingChara.Level, 1)
                    .SetDBValue(startingChara.Enhancement, 0)
                    .SetDBValue(startinCharaItem.Number, 1)
                    .Submit(result =>
                    {
                        if (false == result)
                        {
                            Debug.LogError("프로필 생성에 실패함");
                            return;
                        }

                        Debug.Log("새로운 계정 생성");
                        haveCharacterIdxList.Add(1);
                        onLoadUserDataCompleted?.Invoke();
                        onLoadUserDataCompleted.RemoveAllListeners();
                    });
                return;
            }

            // DB의 데이터를 캐싱

            // 프로필 로딩
            this.Profile.Name.SetValueWithDataSnapshot(userData);
            this.Profile.Level.SetValueWithDataSnapshot(userData);
            this.Profile.IconIndex.SetValueWithDataSnapshot(userData);
            this.Profile.Introduction.SetValueWithDataSnapshot(userData);
            this.Profile.Rank.SetValueWithDataSnapshot(userData);

            this.Profile.MyroomBgIdx.SetValueWithDataSnapshot(userData);
            this.Profile.MyroomCharaIdx.SetValueWithDataSnapshot(userData);

            this.PlayData.EggGainTimestamp.SetValueWithDataSnapshot(userData);
            this.PlayData.IdleRewardTimestamp.SetValueWithDataSnapshot(userData);
            this.PlayData.BatchInfo.SetValueWithDataSnapshot(userData);
            this.PlayData.GoldDungeonClearRate.SetValueWithDataSnapshot(userData);
            this.PlayData.ExpDungeonClearRate.SetValueWithDataSnapshot(userData);
            this.PlayData.EnforceDungeonClearRate.SetValueWithDataSnapshot(userData);

            foreach (var userDataValue in this.PlayData.HasRoom)
            {
                userDataValue.SetValueWithDataSnapshot(userData);
            }
            foreach (var userDataValue in this.PlayData.TutorialDone)
            {
                userDataValue.SetValueWithDataSnapshot(userData);
            }
            

            // 캐릭터 데이터 로딩
            if (userData.HasChild("Characters"))
            {
                DataSnapshot allCharacterData = userData.Child("Characters");

                // 키 값 조회 == 보유 캐릭터 확인
                foreach (DataSnapshot singleCharacterData in allCharacterData.Children)
                {
                    // DB에서 가져온 키값 문자열 int로 파싱하기 vs 문자열을 키값으로 쓰기
                    if (false == int.TryParse(singleCharacterData.Key, out int id))
                    {
                        Debug.LogWarning($"잘못된 키 값({singleCharacterData.Key})");
                        continue;
                    }

                    CharacterData characterData = GameManager.TableData.GetCharacterData(id);
                    haveCharacterIdxList.Add(id);

                    GetHasCharacterData(id).SetValueWithDataSnapshot(userData);
                    GetCharacterLevel(id).SetValueWithDataSnapshot(userData);
                    GetCharacterEnhancement(id).SetValueWithDataSnapshot(userData);
                    GetCharacterMileage(id).SetValueWithDataSnapshot(userData);
                }

            }


            // 아이템 데이터 로딩
            if (userData.HasChild("Items"))
            {
                DataSnapshot allItemData = userData.Child("Items");

                // 키 값 조회 == 보유 아이템 확인
                foreach (DataSnapshot singleItemData in allItemData.Children)
                {
                    // DB에서 가져온 키값 문자열 int로 파싱하기 vs 문자열을 키값으로 쓰기
                    if (false == int.TryParse(singleItemData.Key, out int id))
                    {
                        Debug.LogWarning($"잘못된 키 값({singleItemData.Key})");
                        continue;
                    }

                    GetItemNumber(id).SetValueWithDataSnapshot(userData);
                }
            }

            // 스테이지 데이터 로딩
            if (userData.HasChild("Stages"))
            {
                DataSnapshot allStageData = userData.Child("Stages");

                // 키 값 조회 == 스테이지 기록 존재 여부 확인
                foreach (DataSnapshot singleStageData in allStageData.Children)
                {
                    // DB에서 가져온 키값 문자열 int로 파싱하기 vs 문자열을 키값으로 쓰기
                    if (false == int.TryParse(singleStageData.Key, out int id))
                    {
                        Debug.LogWarning($"잘못된 키 값({singleStageData.Key})");
                        continue;
                    }

                    GetStageClearCount(id).SetValueWithDataSnapshot(userData);
                }
            }

            // 상점 데이터 로딩
            if (userData.HasChild("ShopItems"))
            {
                DataSnapshot allShopItemData = userData.Child("ShopItems");

                // 키 값 조회 == 구매 기록 존재 여부 확인
                foreach (DataSnapshot singleStageData in allShopItemData.Children)
                {
                    // DB에서 가져온 키값 문자열 int로 파싱하기 vs 문자열을 키값으로 쓰기
                    if (false == int.TryParse(singleStageData.Key, out int id))
                    {
                        Debug.LogWarning($"잘못된 키 값({singleStageData.Key})");
                        continue;
                    }

                    GetPackageBought(id).SetValueWithDataSnapshot(userData);
                }
            }

            onLoadUserDataCompleted?.Invoke();
            onLoadUserDataCompleted.RemoveAllListeners();
        });

    }

    /// <summary>
    /// UID를 전달받아 해당 유저의 Profile을 가져옴[비동기].
    /// </summary>
    /// <param name="othersUID">가져올 유저의 UID</param>
    /// <param name="callback">Profile을 반환받을 콜백함수</param>
    public void GetOtherUserProfileAsync(string othersUID, Action<UserProfile> callback)
    {
        if (Application.isPlaying == false)
        {
            Debug.LogWarning("플레이모드가 아닐 경우 오작동할 수 있습니다");
        }

        BackendManager.AllUsersDataRef.Child(othersUID).GetValueAsync().ContinueWithOnMainThread(result =>
        {

            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.Log("데이터베이스 접근 실패");
                return;
            }

            DataSnapshot profileSnapshot = result.Result;

            if (null == profileSnapshot)
            {
                Debug.Log("해당 UID의 유저가 존재하지 않음");
                return;
            }

            UserProfile otherProfile = new UserProfile();

            otherProfile.Name.SetValueWithDataSnapshot(profileSnapshot);
            otherProfile.IconIndex.SetValueWithDataSnapshot(profileSnapshot);
            otherProfile.Level.SetValueWithDataSnapshot(profileSnapshot);
            otherProfile.Introduction.SetValueWithDataSnapshot(profileSnapshot);
            otherProfile.Rank.SetValueWithDataSnapshot(profileSnapshot);

            otherProfile.MyroomBgIdx.SetValueWithDataSnapshot(profileSnapshot);
            otherProfile.MyroomCharaIdx.SetValueWithDataSnapshot(profileSnapshot);

            callback?.Invoke(otherProfile);
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

        /// <summary>
        /// DB에서 관리되는 유저 데이터
        /// </summary>
        /// <typeparam name="T">Firebase에서 관리되는 타입(string, long, double, bool 등) 사용</typeparam>
        public class PropertyAdapter<T> where T : System.IEquatable<T>
        {
            public T Value { get; private set; }

            public event UnityAction<T> onValueChanged;

            public string Key { get; private set; }

            public PropertyAdapter(string key, T defaultValue)
            {
                this.Key = key;
                this.Value = defaultValue;
            }

            /// <summary>
            /// 로딩 단계에서 초기값 입력을 위한 메서드
            /// </summary>
            /// <param name="userDataSnapshot">대상 유저의 데이터스냅샷</param>
            public void SetValueWithDataSnapshot(DataSnapshot userDataSnapshot)
            {
                object value = userDataSnapshot.Child(Key).Value;
                if (value != null)
                    this.Value = (T)value;
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

            /// <summary>
            /// UpdateDbChain.SetDBValue()에서 현재 서버 시간 갱신 등록을 위한 메서드<br/>
            /// 다른 용도의 사용을 상정하지 않음
            /// </summary>
            /// <param name="updateDbChain"></param>
            public void RegisterTimestampToChain(UpdateDbChain updateDbChain)
            {
                updateDbChain.updates[Key] = ServerValue.Timestamp;
                updateDbChain.propertyCallbackOnSubmit += GetValueRequest;
            }

            private void GetValueRequest() // DB에 해당 값 요청
            {
                BackendManager.CurrentUserDataRef.Child(Key).GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        GetValueRequest(); // 재요청
                        Debug.LogWarning($"요청 실패함, 재 요청 전송됨");
                        return;
                    }

                    T dbValue = (T)task.Result.Value;
                    if (false == this.Value.Equals(dbValue))
                    {
                        this.Value = dbValue;
                        onValueChanged?.Invoke(dbValue);
                    }
                });
            }
        }

        public class DictionaryAdapter<T>
        {
            public Dictionary<string, T> Value { get; private set; }

            public event UnityAction onValueChanged;

            public string Key { get; private set; }

            public DictionaryAdapter(string key)
            {
                this.Key = key;
                Value = new Dictionary<string, T>();
            }

            /// <summary>
            /// 로딩 단계에서 초기값 입력을 위한 메서드
            /// </summary>
            /// <param name="userDataSnapshot">대상 유저의 데이터스냅샷</param>
            public void SetValueWithDataSnapshot(DataSnapshot userDataSnapshot)
            {
                object value = userDataSnapshot.Child(Key).Value;
                //Dictionary<string, object> tempDict = value as Dictionary<string, object>;

                DataSnapshot tempDict = userDataSnapshot.Child(Key);

                if (tempDict != null) 
                {

                    Value = new Dictionary<string, T>((int)tempDict.ChildrenCount << 1);

                    foreach (var data in tempDict.Children)
                    {
                        Value[data.Key] = (T)data.Value;
                    }
                }
            

/*                if (tempDict != null)
                {
                    this.Value = new Dictionary<string, T>(tempDict.Count << 1);
                    foreach (var pair in tempDict)
                    {
                        this.Value[pair.Key] = (T)pair.Value;
                    }
                }*/
            }

            /// <summary>
            /// UpdateDbChain.SetDBValue()에서 갱신 대상 등록을 위한 메서드<br/>
            /// 다른 용도의 사용을 상정하지 않음
            /// </summary>
            /// <param name="updateDbChain"></param>
            /// <param name="value"></param>
            public void RegisterToChain(UpdateDbChain updateDbChain, Dictionary<string, T> value) // UpdateDbChain 바깥에선 숨기는 방법이 없을까?
            {
                updateDbChain.updates[Key] = value;
                updateDbChain.propertyCallbackOnSubmit += () =>
                {
                    Value = value;
                    onValueChanged?.Invoke();
                };
            }

            /// <summary>
            /// UpdateDbChain.SetDBValue()에서 갱신 대상 등록을 위한 메서드<br/>
            /// 다른 용도의 사용을 상정하지 않음
            /// </summary>
            /// <param name="updateDbChain"></param>
            /// <param name="value"></param>
            public void RegisterToDBValueChain(UpdateDbChain updateDbChain, string childKeyValue, T value)
            {
                updateDbChain.updates[$"{Key}/{childKeyValue}"] = value;
                updateDbChain.propertyCallbackOnSubmit += () =>
                {
                    Value[childKeyValue] = value;
                    //onValueChanged?.Invoke();//호출 안함 [ 임시조치 ]
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

        public UpdateDbChain AddDBValue(PropertyAdapter<long> property, int value)
        {
#if DEBUG
            if (updates.ContainsKey(property.Key))
            {
                Debug.LogWarning("한 스트림에 데이터를 두번 갱신하고 있음");
            }
#endif //DEBUG
            property.RegisterToChain(this, property.Value + value);

            return this;
        }

        [Obsolete]
        public UpdateDbChain AddDBValue(PropertyAdapter<double> property, float value)
        {
#if DEBUG
            if (updates.ContainsKey(property.Key))
            {
                Debug.LogWarning("한 스트림에 데이터를 두번 갱신하고 있음");
            }
#endif //DEBUG
            property.RegisterToChain(this, property.Value + value);

            return this;
        }

        public UpdateDbChain SetDBDictionary<T>(DictionaryAdapter<T> property, Dictionary<string, T> value)
        {
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
        /// 딕셔너리 타입의 내부 요소 일부를 수정하기 위한 함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">갱신할 대상 딕셔너리</param>
        /// <param name="keyValue">갱신할 딕셔너리의 키</param>
        /// <param name="value">갱신할 값</param>
        /// <returns></returns>
        public UpdateDbChain SetDBDictionaryInnerValue<T>(DictionaryAdapter<T> property, in string keyValue, T value)
        {
#if DEBUG
            if (updates.ContainsKey($"{property.Key}/{keyValue}"))
            {
                Debug.LogWarning("한 스트림에 데이터를 두번 갱신하고 있음");
            }
#endif //DEBUG
            property.RegisterToDBValueChain(this, keyValue, value);

            return this;
        }

        public UpdateDbChain SetDBTimestamp(UserDataDateTime property)
        {
#if DEBUG
            if (updates.ContainsKey(property.Key))
            {
                Debug.LogWarning("한 스트림에 데이터를 두번 갱신하고 있음");
            }
#endif //DEBUG

            property.RegisterTimestampToChain(this);

            return this;
        }

        /// <summary>
        /// DB경로를 직접 지정하여 값을 갱신
        /// </summary>
        /// <typeparam name="T">갱신될 값의 타입</typeparam>
        /// <param name="databasePath">데이터베이스의 경로 [Users/uid]기준 경로</param>
        /// <param name="value">갱신될 값</param>
        /// <returns></returns>
        public UpdateDbChain SetDBValue<T>(string databasePath, T value)
        {
#if DEBUG
            if (updates.ContainsKey(databasePath))
            {
                Debug.LogWarning("한 스트림에 데이터를 두번 갱신하고 있음");
            }
#endif //DEBUG

            updates[databasePath] = value;

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

            GameManager.Instance.StartShortLoadingUI();
            BackendManager.CurrentUserDataRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
            {
                GameManager.Instance.StopShortLoadingUI();
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"요청 실패함");
                    onCompleteCallback?.Invoke(false);
                    return;
                }

                Debug.Log($"데이터 갱신 성공");
                propertyCallbackOnSubmit?.Invoke();
                onCompleteCallback?.Invoke(true);
            });

        }
    }
    #endregion DB 데이터 갱신
}

public class UserDataInt : UserDataManager.UpdateDbChain.PropertyAdapter<long>
{
    public new int Value => (int)base.Value;

    public UserDataInt(string key, int defaultValue = default) : base(key, defaultValue) { }
}

//public class UserDataFloat : UserDataManager.UpdateDbChain.PropertyAdapter<double>
//{
//    public new float Value => (float)base.Value;

//    public UserDataFloat(string key, float defaultValue = default) : base(key, defaultValue) { }
//}

public class UserDataString : UserDataManager.UpdateDbChain.PropertyAdapter<string>
{
    public UserDataString(string key, string defaultValue = default) : base(key, defaultValue) { }
}

public class UserDataDateTime : UserDataManager.UpdateDbChain.PropertyAdapter<long>
{
    public new DateTime Value => new DateTime(1970, 1, 1, 9/*UTC+9*/, 0, 0, DateTimeKind.Utc).AddMilliseconds(base.Value);

    public UserDataDateTime(string key) : base(key, 0) { }
}

public class UserDataDictionaryLong : UserDataManager.UpdateDbChain.DictionaryAdapter<long>
{
    public UserDataDictionaryLong(string key) : base(key) { }
}