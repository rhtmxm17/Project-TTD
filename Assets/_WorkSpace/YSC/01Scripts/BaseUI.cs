using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectDic;
    private Dictionary<(string, System.Type), Component> componentDic;

    protected virtual void Awake()
    {
        Bind();
    }
    private void Bind()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);
        componentDic = new Dictionary<(string, System.Type), Component>();
        foreach (Transform child in transforms)
        {
            // 혹시 이름 겹쳐서 추가 안되면 알 수 있도록 로그 띄우기.
            // bool isSuccess =  gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
            // if (isSuccess == false)
            // {
            //     Debug.LogWarning($"이미 {child.gameObject.name} Object가 있어서 추가되지 않습니다. ");
            // }

            if (gameObjectDic.ContainsKey(child.gameObject.name))
            {
               // Debug.LogWarning($"이미 {child.gameObject.name} Object가 있어서 추가되지 않습니다. ");
                continue;
            }
            gameObjectDic[child.gameObject.name] = child.gameObject;
        }
    }

    // 이름이 name인 UI 게임오브젝트 가져오기
    // GetUI("Key") : Key 이름의 게임오브젝트 가져오기
    public GameObject GetUI(in string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        return gameObject;
    }
    // 이름이 name인 UI에서 컴포넌트 T 가져오기
    // GetUI<Image>("Key") : Key 이름의 게임오브젝트에서 Image 컴포넌트 가져오기.
    public T GetUI<T>(in string name) where T : Component
    {
        // Ex) Button 게임오브젝트 안에 Image 컴포넌트의 키 : Button_Image
        // Ex) Chest 게임오브젝트 안에 Transform 컴포넌트의 키 : Chest_Transform
        (string, System.Type) key = (name, typeof(T)); // 반복사용되니까 key 캐싱

        // 1. Component 딕셔너리에 이미 있을때(찾아본적이 있는경우) : 이미 찾았던거 주기
        componentDic.TryGetValue(key, out Component component);
        if (component != null)
            return component as T;

        // 2. Component 딕셔너리에 아직 없을때(찾아본적이 없는경우) : 찾은 후 딕셔너리 추가.(Binding)
        // gameObject 그 이름의 게임오브젝트가 없을수도 있으니 찾아보고
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)  // 없으면 반환
            return null;

        // gameObject 찾아보고
        component = gameObject.GetComponent<T>();
        if (component == null) // 컴포넌트에 없었으면 반환 하징낳기
            return null;

        componentDic.TryAdd(key, component); // 잘찾으면 T로 반환 
        return component as T;
    }

}
