using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    // 능력치 표 내지는 계산식으로 변경 필요함
    [System.Serializable]
    public struct Status
    {
        public int Range;

    }

    [SerializeField] int id;
    public int Id => id;

    [SerializeField] GameObject modelPrefab;
    public GameObject ModelPrefab => modelPrefab;

    [SerializeField] Status statusTable;
    public Status StatusTable => statusTable;

    [SerializeField] Sprite skillSprite;
    public Sprite SkillSprite => skillSprite;

    [SerializeField] Sprite faceIconSprite;
    public Sprite FaceIconSprite => faceIconSprite;
}
