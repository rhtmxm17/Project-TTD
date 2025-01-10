using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class KMT_BaseUI : BaseUI
{

    [SerializeField] HYJ_CharacterSelect buttonPrefab;

    Transform buttonsParent;
    HYJ_SelectManager selectManager;
    HYJ_ListController listController;
    GameObject unitChangePanel;

    DatabaseReference userUidRef;

    private void Start()
    {
        userUidRef = BackendManager.CurrentUserDataRef;
        Debug.Log(userUidRef == null);

        selectManager = GetComponent<HYJ_SelectManager>();
        listController = GetComponent<HYJ_ListController>();
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

            Debug.Log("가져온 보유 캐릭터 수"+task.Result.ChildrenCount);

            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot chInfo in snapshot.Children)
            {
                HYJ_CharacterSelect button = Instantiate(buttonPrefab, buttonsParent);
                int.TryParse(chInfo.Key, out int parsd);
                //Debug.Log("TP" + int.TryParse(chInfo.Key,out int Parsd));
                //int.TryParse(chInfo.ket, out int Parsd);
                //Debug.Log(Parsd);
                button.InitDataUnitBtn(selectManager, parsd, unitChangePanel);
                //listController.AddListUnit(parsd);
            }
        });
    }
}
