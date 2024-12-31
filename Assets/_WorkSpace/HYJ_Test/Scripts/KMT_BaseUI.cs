using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMT_BaseUI : BaseUI
{

    [SerializeField]
    HYJ_CharacterSelect buttonPrefab;

    Transform buttonsParent;
    HYJ_SelectManager selectManager;
    GameObject unitChangePanel;

    DatabaseReference userUidRef;

    private void Start()
    {
        //yield return new WaitUntil(() => { return GameManager.Database != null; });
        //yield return new WaitUntil(() => { return GameManager.Database.RootReference != null; });
        //yield return StartCoroutine(UserDataManager.InitDummyUser(7));

        userUidRef = BackendManager.CurrentUserDataRef;
        Debug.Log(userUidRef == null);

        selectManager = GetComponent<HYJ_SelectManager>();

        buttonsParent = GetUI<Transform>("Content_Characters");
        unitChangePanel = GetUI<Transform>("UnitChangePanel").gameObject;

        SettingMyCharacters();

    }

    private void SettingMyCharacters()
    {
        userUidRef.Child("Characters").OrderByKey().GetValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled) {
                Debug.Log("보유 캐릭터 불러오기 실패");
                return;
            }

            Debug.Log(task.Result.ChildrenCount);

            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot chInfo in snapshot.Children)
            {
                HYJ_CharacterSelect button = Instantiate(buttonPrefab, buttonsParent);
                Debug.Log("TP" + int.TryParse(chInfo.Key,out int Parsd));
                Debug.Log(Parsd);
                button.InitData(selectManager, Parsd, unitChangePanel);
            }
        });
    }
}
