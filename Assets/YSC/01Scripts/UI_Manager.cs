using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : BaseUI
{
    [SerializeField] Button stageButton;



    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // 로긴패널
        GetUI("LoginPanel");
        GetUI<Button>("LobbyButton").onClick.AddListener(() => Open("LobbyPanel"));
        GetUI<Button>("ProfileButton").onClick.AddListener(() => Open("ProfilePopUp"));

        // 프로필팝업
        GetUI<Button>("ProfileBackButton").onClick.AddListener(() => GoBack("ProfilePopUp"));


        // 로비패널
        GetUI("LobbyPanel");
        GetUI<TMP_Text>("TestText").text = "Tear To Dragon 티어 투 드래곤";
        // 스테이지버튼
        stageButton = GetUI<Button>("StageButton");
        stageButton.onClick.AddListener(() => Open("StagePanel"));
        // 팝업버튼 // 변수선언안해도 잘되는구만
        GetUI<Button>("PopUpButton").onClick.AddListener(() => Open("PopUpPanel"));
        GetUI<TMP_Text>("PopUpButtonText").text = "팝업";
        // 팝업패널
        GetUI("PopUpPanel").SetActive(false);
        GetUI<TMP_Text>("PopUpText").text = "팝업창 짜잔";
        // Stage패널
        GetUI("StagePanel").SetActive(false);
        // 뒤로가기 버튼
        GetUI<Button>("BackButton").onClick.AddListener(() => GoBack("PopUpPanel"));


        // 스테이지패널
        GetUI<Button>("StageBackButton").onClick.AddListener(() => GoBack("StagePanel")); 



    }

    /// <summary>
    /// 패널이름 넣기
    /// gameObject로 되있어야함.
    /// ex)  GetUI("PopUpPanel");
    /// </summary>
    /// <param name="name"></param>
    public void Open(string name)
    {
        Debug.Log($"{name} 패널을 엽니다");
        GetUI(name).SetActive(true);
    }

    public void GoBack(string name)
    {
        Debug.Log($"{name} 패널을 닫습니다");
        GetUI(name).SetActive(false);
        /*뭔가하려다 방식변경
        // 부모오브젝트 가져와서 .SetActive(false)
        // 바로 상위 오브젝트만 꺼버리고 싶은데 최상위가 꺼짐.
        // 아 왜 이게 계속 더 밑에 자식이 안되나 했더니 이거 스크립트가 팝업패널에 있는게 아니라 그러네 
        // 그러면 닫고싶은 패널(from. 패널?? ______패널에서 뒤로가기)같은 형식으로??
        // 근데 어차피 Stack으로 해서 POP으로 하게될거같은디 무튼...
        Transform transform = this.gameObject.transform.parent;
        Debug.Log($"부모오브젝트 이름: {transform.name}");
        Transform transform2 = transform.GetChild(transform.childCount);
        Debug.Log($"{transform2.name}");

      //  Transform parent = transform;
      //  transform.SetParent(parent);
      //  Debug.Log($"{gameObject.name}");
      //  parent.GetChild(parent.childCount-1).gameObject.SetActive(false); // gameObject.SetActive(false);
      //  Debug.Log($"{gameObject.name}");
      //  Debug.Log($"뒤로가기");
        */
    }
}
