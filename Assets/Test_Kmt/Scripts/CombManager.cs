using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CombManager : MonoBehaviour
{

    [SerializeField]
    List<GameObject> charList;

    public UnityEvent ListClearedEvent = new UnityEvent();

    /// <summary>
    /// 2024.12.17 백선명 수정
    ///     - Character Data 설정 받아오는 코드 추가
    /// </summary>

    #region InitTest 
    [SerializeField] private List<Image> skillImageList;
    [SerializeField] private List<CharacterData> characterDataList;
    
    private void Start()
    {
        if (characterDataList.Count < 1 || skillImageList.Count < 1)
            return;
        
        SpawnCharacterDataSet();
    }

    private void SpawnCharacterDataSet()
    {

        for (int i = 0; i < charList.Count; i++)
        {
            GameObject instance = Instantiate(characterDataList[i].ModelPrefab, charList[i].transform.position, Quaternion.identity);
            instance.transform.SetParent(charList[i].transform);  
            charList[i].GetComponent<SpriteRenderer>().sprite = characterDataList[i].FaceIconSprite;

            skillImageList[i].sprite = characterDataList[i].SkillSprite;
            
            CharacterCombatable combatable = charList[i].GetComponent<CharacterCombatable>();
            
            skillImageList[i].GetComponentInParent<Button>().onClick.AddListener(()=>combatable.OnSkillCommanded(combatable.gameObject.name));
            
        }
    }
 
    #endregion

    public Transform GetNearestTrackable(Transform fromTransform)
    {
        return GetNearestTrackable(fromTransform.position);
    }
    public Transform GetNearestTrackable(Vector3 fromPos)
    {
        if (charList.Count == 0)
            return null;

        Transform ret = null;
        float minDist = float.MaxValue;

        foreach (GameObject trObj in charList)
        {
            float curDist = Vector3.SqrMagnitude(fromPos - trObj.transform.position);

            if (curDist < minDist)
            {
                minDist = curDist;
                ret = trObj.transform;
            }
        }

        return ret;
    }


    public Transform GetFarestTrackable(Transform fromTransform)
    {
        return GetFarestTrackable(fromTransform.position);
    }
    public Transform GetFarestTrackable(Vector3 fromPos)
    {
        if (charList.Count == 0)
            return null;

        Transform ret = null;
        float maxDist = -1;

        foreach (GameObject trObj in charList)
        {
            float curDist = Vector3.SqrMagnitude(fromPos - trObj.transform.position);

            if (curDist > maxDist)
            {
                maxDist = curDist;
                ret = trObj.transform;
            }
        }

        return ret;
    }

    public void OnDead(GameObject deadTrObj)
    {
        charList.Remove(deadTrObj);
        Destroy(deadTrObj);//또는 사망처리 + 비활성화.

        if (charList.Count <= 0)
        {
            //리스트가 비었을 때 ( 항목으 캐릭터가 모두 죽었을 경우 호출될 이벤트 )
            ListClearedEvent?.Invoke();
        }

    }
}
