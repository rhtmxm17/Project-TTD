using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMType { TEST1, TEST2, TEST3 }

[System.Serializable]
public struct BGM
{
    [SerializeField]
    public BGMType Type;
    [SerializeField]
    public AudioClip AudioClip;
}

public class BGMDictionary : MonoBehaviour
{
    [SerializeField]
    public BGM[] BGMArray;

    Dictionary<BGMType, AudioClip> audioDictionary = new Dictionary<BGMType, AudioClip>();

    private void Awake()
    {
        foreach (BGM bgm in BGMArray)
        {
            audioDictionary.Add(bgm.Type, bgm.AudioClip);
        }
    }

    /// <summary>
    /// bgmtype [enum]을 받아서 대응하는 audioClip을 반환
    /// </summary>
    /// <param name="bgmType">얻어올 bgm의 enum</param>
    /// <returns>대응되는 audioClip, 없을경우 null반환</returns>
    public AudioClip GetAudioClip(BGMType bgmType)
    {
        if (audioDictionary.ContainsKey(bgmType))
        { 
            return audioDictionary[bgmType];
        }

        return null;
    }

}
