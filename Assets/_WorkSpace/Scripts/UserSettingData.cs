using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 디바이스에 저장되는, 설정값과 같은 데이터 관리자
/// </summary>
public class UserSettingData : SingletonScriptable<UserSettingData>
{
    public SettingData Data => data;
    private SettingData data;
    private string path;

    [SerializeField] SettingData defaultData; // 기본값은 SO 애셋으로 입력

    /// <summary>
    /// 실제로 저장될 데이터 내역
    /// </summary>
    [System.Serializable]
    public class SettingData
    {
        public float[] soundScale = new float[SoundManager.AudioGroupCount];
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        // 에디터 모드에서는 (프로젝트 폴더)/Temp 에 생성
        // 유니티 에디터 종료시 휘발됩니다
        path = $"./Temp/UserSetting.json";
#else
        path = $"{Application.persistentDataPath}/UserSetting.json";
#endif
    }

    public void SaveSetting()
    {
        string jData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jData);
    }

    public void LoadSetting()
    {
        if (File.Exists(path))
        {
            string jData = File.ReadAllText(path);
            data = JsonUtility.FromJson<SettingData>(jData);
            Debug.Log(path);
        }
        else
        {
            // 기본값 직렬화 복사
            data = JsonUtility.FromJson<SettingData>(JsonUtility.ToJson(defaultData, true));
        }
    }
}
