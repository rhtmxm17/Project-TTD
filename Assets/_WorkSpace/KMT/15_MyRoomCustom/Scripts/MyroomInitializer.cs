using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyroomInitializer : MonoBehaviour
{
    BaseUI myroomUI;
    MyRoomUI initTarget;

    DialogueUI dialogueUI = null;

    List<GameObject> ownButtonList = new List<GameObject>();


    public void Initialize(BaseUI roomBase)
    {
        myroomUI = roomBase;
        initTarget = roomBase.GetComponent<MyRoomUI>();

        ownButtonList.Add(myroomUI.GetUI<Transform>("RoomChangeButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("CharacterChangeButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("VisitButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("TimerBox").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("SpawnerButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("Dictionary").gameObject);

        dialogueUI = myroomUI.GetUI<DialogueUI>("ChatPopup");
        ;
    }

    [ContextMenu("InitTest")]
    public void InitRoom(string destUid)
    {
        dialogueUI.SetCurVisitUID(destUid);

        UserDataManager.Instance.GetOtherUserProfileAsync(destUid, (profile) =>
        {
            initTarget.ChangeBackground(profile.MyroomBgIdx.Value);
            initTarget.ChangeCharacter(profile.MyroomCharaIdx.Value);

            if (destUid.Equals(UserData.myUid))
            {
                OnOwnUIs();
            }
            else 
            { 
                OffOwnUIs();
            }

        });
    }

    void OnOwnUIs()
    {
        foreach (var item in ownButtonList)
        {
            item.SetActive(true);
        }
    }

    void OffOwnUIs()
    {
        foreach (var item in ownButtonList)
        {
            item.SetActive(false);
        }
    }

}
