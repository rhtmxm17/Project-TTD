using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    
    Stack<T> poolStack = new Stack<T>();
    T objPrefab = null;

    public ObjectPool(T objPrefab, bool initActivate, int count = 10)
    { 
        poolStack = new Stack<T>();
        this.objPrefab = objPrefab;

        for (int i = 0; i < count; i++)
        {
            T obj = Object.Instantiate(objPrefab);
            obj.gameObject.SetActive(initActivate);
            poolStack.Push(obj);
        }
    }

    public T GetItem()
    {
        if (poolStack.Count <= 0)
            return Object.Instantiate(objPrefab);
        else
            return poolStack.Pop();

    }

    public void BackItem(T item)
    {
        if (item != null)
            poolStack.Push(item);
    }
}

public class DamageDisplayer : MonoBehaviour
{
    [SerializeField]
    DamageTextObj damageTextObjPrefab;

    ObjectPool<DamageTextObj> textPool;
    private void Awake()
    {
        textPool = new ObjectPool<DamageTextObj>(damageTextObjPrefab, false);
    }


    /// <summary>
    /// 데미지 숫자를 표기하는 텍스트 추가
    /// </summary>
    /// <param name="damageAmount">데미지 크기[수치를 소수점 아래 두자리까지 표기]</param>
    /// <param name="isDamage">데미지여부 [ false인 경우 회복 효과 ]</param>
    /// <param name="position">출력할 위치</param>
    public void PlayTextDisplay(float damageAmount, bool isDamage, Vector3 position)
    {
        textPool.GetItem().SetDamageText(damageAmount, isDamage, position, textPool);
    }

}
