using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyroomInitializer : MonoBehaviour
{
    [SerializeField]
    Sprite[] backgroundSprites;

    BaseUI myroomUI;
    Image backImg = null;
    Image charaImg = null;

    DialogueUI dialogueUI = null;

    List<GameObject> ownButtonList = new List<GameObject>();

    public void Initialize(BaseUI roomBase)
    {
        myroomUI = roomBase;

        ownButtonList.Add(myroomUI.GetUI<Transform>("RoomChangeButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("CharacterChangeButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("VisitButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("TimerBox").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("SpawnerButton").gameObject);
        ownButtonList.Add(myroomUI.GetUI<Transform>("Dictionary").gameObject);

        dialogueUI = myroomUI.GetUI<DialogueUI>("ChatPopup");
    }

    [ContextMenu("InitTest")]
    public void InitRoom(string destUid)
    {
        //TODO : 존재여부부터 확인, 있는걸 가정하고 우선 진행
        backImg = myroomUI.GetUI<Image>("BackImage");
        charaImg = myroomUI.GetUI<Image>("MyRoomCharacter");

        dialogueUI.SetCurVisitUID(destUid);

        UserDataManager.Instance.GetOtherUserProfileAsync(destUid, (profile) =>
        {

            backImg.sprite = backgroundSprites[profile.MyroomBgIdx.Value];
            charaImg.sprite = DataTableManager.Instance.GetCharacterData(profile.MyroomCharaIdx.Value).FaceIconSprite;

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
