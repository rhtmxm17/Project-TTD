using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CombManager : MonoBehaviour
{

    [SerializeField]
    public List<Combatable> CharList;

    public UnityEvent ListClearedEvent = new UnityEvent();

    public CombManager Enemy { get; private set; }

    public void StartCombat(CombManager againstGroup)
    {
        Enemy = againstGroup;
        foreach (Combatable character in CharList)
        { 
            character.StartCombat(againstGroup);
        }
    }

    public void EndCombat()
    {
        Enemy = null;
        foreach (Combatable character in CharList)
        {
            character.EndCombat();
        }
    }

    public Transform GetNearestTrackable(Transform fromTransform)
    {
        return GetNearestTrackable(fromTransform.position);
    }
    public Transform GetNearestTrackable(Vector3 fromPos)
    {
        if (CharList.Count == 0)
            return null;

        Transform ret = null;
        float minDist = float.MaxValue;

        foreach (Combatable trObj in CharList)
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
        if (CharList.Count == 0)
            return null;

        Transform ret = null;
        float maxDist = -1;

        foreach (Combatable trObj in CharList)
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

    public void OnDead(Combatable deadTrObj)
    {
        CharList.Remove(deadTrObj);
        Destroy(deadTrObj);//또는 사망처리 + 비활성화.

        if (CharList.Count <= 0)
        {
            //리스트가 비었을 때 ( 항목으 캐릭터가 모두 죽었을 경우 호출될 이벤트 )
            ListClearedEvent?.Invoke();
        }

    }
}