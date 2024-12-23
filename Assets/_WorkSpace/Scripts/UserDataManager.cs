using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase.Database;

public class UserDataManager : SingletonScriptable<UserDataManager>
{
    public UnityEvent onLoadUserDataCompleted;

    public UserProfile Profile { get; private set; } = new UserProfile();

    public class UserProfile
    {
        public UserDataString Name { get; private set; } = new UserDataString($"Profile/Name", "이름 없음");
        public UserDataInt IconIndex { get; private set; } = new UserDataInt($"Profile/IconIndex");
        public UserDataInt Level { get; private set; } = new UserDataInt($"Profile/Level", 1);
        public UserDataString Introduction { get; private set; } = new UserDataString($"Profile/Introduction", "소개문 없음");
    }


    /// <summary>
    /// 테스트용 가인증 코드입니다
    /// </summary>
    /// <param name="DummyNumber">가인증 uid값 뒤쪽에 붙일 번호</param>
    /// <returns></returns>
    public static IEnumerator InitDummyUser(int DummyNumber)
    {
        // Database 초기화 대기
        yield return new WaitWhile(() => GameManager.Database == null);
        BackendManager.Instance.UseDummyUserDataRef(DummyNumber); // 테스트코드
        Instance.LoadUserData();
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

            // DB의 데이터를 캐싱

            // 프로필 로딩
            this.Profile.Name.SetValueWithDataSnapshot(userData);
            this.Profile.Level.SetValueWithDataSnapshot(userData);
            this.Profile.IconIndex.SetValueWithDataSnapshot(userData);
            this.Profile.Introduction.SetValueWithDataSnapshot(userData);

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

                    CharacterData characterData = GameManager.Data.GetCharacterData(id);

                    characterData.Level.SetValueWithDataSnapshot(userData);
                    characterData.Enhancement.SetValueWithDataSnapshot(userData);
                }
            }

            // 캐릭터 데이터 로딩
            if (userData.HasChild("Items"))
            {
                DataSnapshot allItemData = userData.Child("Items");

                // 키 값 조회 == 보유 캐릭터 확인
                foreach (DataSnapshot singleItemData in allItemData.Children)
                {
                    // DB에서 가져온 키값 문자열 int로 파싱하기 vs 문자열을 키값으로 쓰기
                    if (false == int.TryParse(singleItemData.Key, out int id))
                    {
                        Debug.LogWarning($"잘못된 키 값({singleItemData.Key})");
                        continue;
                    }

                    ItemData itemData = GameManager.Data.GetItemData(id);

                    itemData.Number.SetValueWithDataSnapshot(userData);
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

public class UserDataInt : UserDataManager.UpdateDbChain.PropertyAdapter<long>
{
    public new int Value => (int)base.Value;

    public UserDataInt(string key, int defaultValue = default) : base(key, defaultValue) { }
}

public class UserDataFloat : UserDataManager.UpdateDbChain.PropertyAdapter<double>
{
    public new float Value => (float)base.Value;

    public UserDataFloat(string key, float defaultValue = default) : base(key, defaultValue) { }
}

public class UserDataString : UserDataManager.UpdateDbChain.PropertyAdapter<string>
{
    public UserDataString(string key, string defaultValue = default) : base(key, defaultValue) { }
}