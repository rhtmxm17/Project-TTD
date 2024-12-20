using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    // FIXME : 완성 후 전체적으로 리펙토링 및 수정 필요

    // 스토리가 끝나면 스토리 프리팹 삭제
    [SerializeField] GameObject storyPrefab;

    // 불러올 데이터가 있는 곳
    [SerializeField] DialogueData dialogueData;
    
    // 변경할 캐릭터 이미지
    [SerializeField] Image[] characterImage;

    // 추가적으로 컨트롤이 필요할 수 있는 박스에 대한 선언
    [Header("텍스트 박스")]
    [SerializeField] GameObject nameBox;
    [SerializeField] GameObject dialogueBox;

    [Header("텍스트")]
    // 불러온 데이터를 적용 시킬 곳
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;

    // 터치로 넘어가기 위한 Input시스템
    [Header("임시구역(후에 삭제)")]
    [SerializeField] PlayerInput input;
    
    private Dialogue[] dialogues;
    private int dialogueCounter = 0;
    private int maxDialogueCounter;
    private int maxNameCounter;

    [SerializeField]  bool isAuto = false;
    private int autoCounter = 0;

    // 대사의 카운트를 늘릴 변수
    private int nameCounter = 0;

    private void Start()
    {
        // 불러올 데이터를 셋팅
        dialogues = dialogueData.dialogues;
        
        // FIXME : 찾는거 용량 많이 먹을거 같은데....뭔가 수정해야할거 같은데....
        // 근데 또 생각해보면 스토리 프리팹 여러개 열것도 아니고 이거 돌려 쓸건데 그냥 인스펙터 창에서 찾아넣을까 싶기도 하고....
        nameBox = GameObject.Find("Name");
        dialogueBox = GameObject.Find("Dialog");

        maxNameCounter = dialogueData.dialogues.Length;

        // 시작할때 첫 대사
        StoryPrint(nameCounter, dialogueCounter);
        
    }


    private void Update()
    {
        if (input.actions["Click"].WasPressedThisFrame())
        {
            if (EventSystem.current.currentSelectedGameObject == true)
                return;

            TextCount();
            if (nameCounter == maxNameCounter)
            {
                StoryEnd();
                return;
            }
            StoryPrint(nameCounter, dialogueCounter);
        }
        
   
    }

    // 실질적으로 스토리 텍스트를 출력할 함수
    private void StoryPrint(int nameCount, int dialogueCount)
    {
        nameText.text = dialogues[nameCount].name;
        // dialogueText.text = dialogues[nameCount].contexts[dialogueCounter];
        
        dialogueText.text = "";
        
        dialogueText.DOText(dialogues[nameCount].contexts[dialogueCounter],1,true,ScrambleMode.None);
        
        // 출력중 클릭했을 때 진행하던 글자 출력을 완료 하는 함수
        // FIXME : 조건문이 한번 클릭하면 프레임 내내 계속 클릭된걸로 인식.... 어떻게 고쳐야하나
        if (input.actions["Click"].IsPressed())
        {
            // dialogueText.DOComplete();
            
        }
        
    }

    // 텍스트 카운터를 늘려줄 함수
    private void TextCount()
    {
        if (nameCounter < maxNameCounter)
        {
            maxDialogueCounter = dialogueData.dialogues[nameCounter].contexts.Length-1;
            if (dialogueCounter < maxDialogueCounter)
            {
                dialogueCounter++;
            }
            else if (dialogueCounter == maxDialogueCounter)
            {
                nameCounter++;
                dialogueCounter = 0;
            }
        }

        
    }

    // 스토리가 끝났을 때 (임시) 자동으로 창을 닫는 함수
    private void StoryEnd()
    {
        // TODO : 스토리 끝난 이후 할 행동에 대한 구현
        // 기획 의도에 따라 자동으로 다음 스토리로 넘어갈지, 혹은 전투씬에서 스토리를 부를 경우 창을 바로 없앨지 등에 고민
        Debug.Log("스토리 출력 끝");
        dialogueText.text = "-다음이시간에-";
        // (임시)스토리가 끝나면 창을 자동으로 닫음
        Destroy(storyPrefab,5f);
    }

    // 스토리 진행시 캐릭터의 이미지를 변경할 함수
    private void ChangeCharacterImage()
    {
        // FIXME : 나중에는 전투에서 캐릭터 이미지 불러오듯이 수정
        // 이름 텍스트와 같은 캐릭터의 이미지 출력(임시)
        
        
    }

}