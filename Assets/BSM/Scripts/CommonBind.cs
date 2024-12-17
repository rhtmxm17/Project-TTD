using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonBind : MonoBehaviour
{ 
    private Dictionary<string, GameObject> _goDict;
    private Dictionary<(string, System.Type), Component> _comDict;

    protected void Bind()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        
        _goDict = new Dictionary<string, GameObject>(transforms.Length << 2);
        
        foreach (Transform t in transforms)
        {
            _goDict.TryAdd(t.gameObject.name, t.gameObject); 
        }
        
        _comDict = new Dictionary<(string, System.Type), Component>();
        
    }

    protected GameObject GetCommonGameObject(in string key)
    {
        if (_goDict.TryGetValue(key, out GameObject go)) ;

        return go;
    }

    protected T GetCommonComponent<T>(in string key) where T : Component
    {
        (string, System.Type) _key = (key,typeof(T));

        _comDict.TryGetValue(_key, out Component _com);

        if (_com != null)
            return _com as T;
        
        _goDict.TryGetValue(key, out GameObject _go);

        if (_go != null)
        {
            _com = _go.GetComponent<T>();

            if (_com != null)
            {
                _comDict.TryAdd(_key, _com);
                return _com as T;
            }
        }
        
        return null; 
    }    
}
