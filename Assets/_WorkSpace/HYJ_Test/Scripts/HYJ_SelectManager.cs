using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UserDataManager;

public class HYJ_SelectManager : MonoBehaviour
{
    [SerializeField] public int curPos;
    [SerializeField] public int curUnitIndex;

    [Header("buttonList")]
    [SerializeField]
    List<Transform> buttonsTransformList;

    [Header("이미지 프리팹")]
    [SerializeField] public GameObject ChImagePrefab;

    [Header("spriteTest")]
    [SerializeField]
    List<HYJ_PlayerController> spriteList;

    // 키 값은 위치 / 밸류 값은 유닛 고유번호;
    public Dictionary<int, int> battleInfo = new Dictionary<int, int>();

    private void Start()
    {
        GameManager.UserData.PlayData.BatchInfo.onValueChanged += (() =>
        {
            Debug.Log("편성 정보가 갱신됨");
        });
        GameManager.UserData.onLoadUserDataCompleted.AddListener(() =>
        {
            Debug.Log("유저 정보 불러오기 완료 확인");
        });
    }

    public void LookLog()
    {
        DatabaseReference baseref = BackendManager.CurrentUserDataRef;

        Dictionary<string, long> updates = new Dictionary<string, long>();
        foreach(var pair in battleInfo)
        {
            updates[pair.Key.ToString()] = pair.Value;
        }

        GameManager.UserData.StartUpdateStream()
            .SetDBDictionary(GameManager.UserData.PlayData.BatchInfo, updates)
            .Submit(result =>
            {
                Debug.Log($"요청 결과:{result}");
            });
    }

    public void SetCharacterImage(int posIdx, int charIdx)
    {
        var chImage = Instantiate(ChImagePrefab, buttonsTransformList[posIdx]);
        CharacterData chData = GameManager.TableData.GetCharacterData(charIdx);
        chImage.transform.localPosition = Vector3.zero;
        chImage.GetComponent<Image>().sprite = chData.FaceIconSprite;
        chImage.GetComponent<HYJ_CharacterImage>().curPos = posIdx;
        chImage.GetComponent<HYJ_CharacterImage>().unitIndex = charIdx;
    }

    public void RemoveCharacterImage(int posIdx)
    {
        Destroy(buttonsTransformList[posIdx].GetComponentInChildren<HYJ_CharacterImage>().gameObject);
    }

    public void ChangeImagePos(int fromIdx, int destIdx)
    {
        if (buttonsTransformList[destIdx].GetComponentInChildren<HYJ_CharacterImage>() != null)
        {
            RemoveCharacterImage(destIdx);
        }
        buttonsTransformList[fromIdx].GetComponentInChildren<HYJ_CharacterImage>().transform.SetParent(buttonsTransformList[destIdx].transform);

        HYJ_CharacterImage[] transL = buttonsTransformList[destIdx].GetComponentsInChildren<HYJ_CharacterImage>();
        RectTransform trans = transL[transL.Length - 1].GetComponent<RectTransform>();
        trans.anchoredPosition = Vector3.zero;
    }

    public void TestLog()
    {
        // 간편 테스트용 / 파이어 베이스 연결x
        Debug.Log("-------");
        foreach (var (key, value) in battleInfo)
        {
            Debug.Log($"{key} : {value}");
        }
        Debug.Log("-------");
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("DamageCalc_kmt");
    }

    public void LoadUserFormation()
    {
        // TODO : 유저가 가지고 있는 포메이션 정보 가져오기

    }

    public void UpdateFormation()
    {
        if (battleInfo.Count > 0)
        {
            foreach (var index in battleInfo)
            {
                //SelectM.battleInfo
            }
        }
    }
}
