using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyroomInitializer : MonoBehaviour
{
    [SerializeField]
    Sprite[] backgroundSprites;

    [SerializeField]
    string othersUID;//Dummy101


    BaseUI myroomUI;
    Image backImg = null;
    Image charaImg = null;

    private void Awake()
    {
        myroomUI = GetComponent<BaseUI>();
    }


    [ContextMenu("InitTest")]
    public void InitRoom()
    {
        backImg = myroomUI.GetUI<Image>("BackImage");
        charaImg = myroomUI.GetUI<Image>("MyRoomCharacter");

        UserDataManager.Instance.GetOtherUserProfileAsync(othersUID, (profile) =>
        {

            backImg.sprite = backgroundSprites[profile.MyroomBgIdx.Value];
            charaImg.sprite = DataTableManager.Instance.GetCharacterData(profile.MyroomCharaIdx.Value).FaceIconSprite;

        });
    }

}
