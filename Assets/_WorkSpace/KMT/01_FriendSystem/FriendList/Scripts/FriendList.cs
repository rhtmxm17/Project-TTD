using Firebase.Database;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FriendList : MonoBehaviour
{

    public const int MAX_FRIEND_CNT = 10;

    [SerializeField]
    FriendBlock friendBlockPrefab;
    [SerializeField]
    Transform contentTransform;

    [SerializeField]
    // OpenableWindow friendPanel;
    private GameObject friendPanel;

    [SerializeField]
    MyroomInitializer initializer;

    [SerializeField]
    TextMeshProUGUI friendCountText;

    DatabaseReference userRef;
    
    // SHW)놀러간 친구의 방 이름 표시
    [SerializeField] private TMP_Text friendRoomText;
    // 내방으로 돌아가기
    [SerializeField] private GameObject returnRoomButton;
    // 스택제거용
    [SerializeField] OutskirtsUI outskirtsUI;
    // 캐릭터 상호작용 비활성용
    [SerializeField] private Button canInteract;

    private void Awake()
    {
        GetComponent<OpenableWindow>().onOpenAction += RefreshList;
    }
    
    // 처음 화면 열때 새로고침 용도
    private void OnEnable()
    {
        RefreshList();
    }

    [ContextMenu("refresh")]
    public void RefreshList()
    {
        userRef = GameManager.Database.RootReference.Child("Users");

        userRef.GetValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("읽어올 수 없음, 친구 목록");
                return;
            }

            FriendBlock[] blocks = contentTransform.GetComponentsInChildren<FriendBlock>();

            foreach (FriendBlock block in blocks)
            {
                Destroy(block.gameObject);
            }

            if (!task.Result.HasChild($"{UserData.myUid}/friends/friendList"))
            {
                Debug.Log("친구 없음");
                friendCountText.text = $"0/{MAX_FRIEND_CNT}";
                return;
            }

            DataSnapshot myFriends = task.Result.Child($"{UserData.myUid}/friends/friendList");

            Debug.Log(myFriends.ChildrenCount + " : 친구수");
            friendCountText.text = $"{myFriends.ChildrenCount}/{MAX_FRIEND_CNT}";

            foreach (DataSnapshot friendsUid in myFriends.Children)
            {

                if (!task.Result.HasChild(friendsUid.Key))
                {
                    Debug.Log("해당 친구가 존재하지 않음");
                    continue;
                }

                Debug.Log(task.Result.Child($"{friendsUid.Key}/Profile/Name").Value);

                string nickname = task.Result.Child($"{friendsUid.Key}/Profile/Name").Value.ToString();

                Instantiate(friendBlockPrefab, contentTransform)
                    .InitData(friendsUid.Key,
                              nickname,
                              this,
                              (str) => {

                                  ItemData friendTicket = DataTableManager.Instance.GetItemData(10/*친구방문 보상 수령 카운트*/);
                                  ItemData gold = DataTableManager.Instance.GetItemData(1/*골드*/);

                                  if (!task.Result.Child($"{UserData.myUid}/friends/visitedList").HasChild(friendsUid.Key)
                                     && friendTicket.Number.Value > 0)
                                  {//보상 받을수 있는 방문
                                      GameManager.UserData.StartUpdateStream()
                                        .AddDBValue(friendTicket.Number, -1)
                                        .AddDBValue(gold.Number, 100)
                                        .SetDBValue($"friends/visitedList/{friendsUid.Key}", "")
                                        .Submit((result) => {

                                            if (result)
                                            {
                                                friendRoomText.text = $"{nickname}님의 방";
                                                // 임시) 내방으로 돌아가기 버튼
                                                returnRoomButton.SetActive(true);
                                                // 캐릭터 상호작용 비활성
                                                canInteract.enabled = false;
                                                // 뒤로가기 버튼 비활성
                                                outskirtsUI.ReturnButton.enabled = false;
                                                 VisitFriend(str, $"{nickname}님의 방이에요! \n 비싼 물건을 찾아보죠!", 
                                                     () => {
                                                         GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                                                              $"100골드정도는 가져가도 괜찮겠죠! \n (경찰이 오기까지 [{friendTicket.Number.Value}/10])",
                                                              "ㄹㅇㄹㅇ",
                                                              null
                                                        );
                                                     });
                                            }
                                            else
                                            {
                                                Debug.Log("놀러가기 실패.");
                                            }

                                        });
                                  }
                                  else
                                  {
                                      friendRoomText.text = $"{nickname}님의 방";
                                      // 임시) 내방으로 돌아가기 버튼
                                      returnRoomButton.SetActive(true);
                                      // 캐릭터 상호작용 비활성
                                      canInteract.enabled = false;
                                      // 뒤로가기 버튼 비활성
                                      outskirtsUI.ReturnButton.enabled = false;
                                      VisitFriend(str, $"{nickname}님의 방이에요! \n 오늘은 이제 구경만 하도록 하죠.", null);
                                  }
                              });

            }

        });

    }

    void VisitFriend(in string uid, in string popupText, Action closeWindowCallback)
    {
        initializer.InitRoom(uid);
        friendPanel.SetActive(false);       // SHW
        GameManager.OverlayUIManager.OpenSimpleInfoPopup(
              popupText,
              "....?",
              closeWindowCallback
        );
    }

}
