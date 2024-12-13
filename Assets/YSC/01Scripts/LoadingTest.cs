using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadingTest : MonoBehaviour
{
    [SerializeField] List<CharacterData> characterDatas;

    private void Awake()
    {
        CharacterData character1 = new CharacterData();
        character1.id = 1;
        character1.name = "클라우드 기사";
        character1.hp = 100;
        character1.level = 1;
        character1.image = Resources.Load<Sprite>("Sprite/Enemy/Enemy1");
        character1.prefab = Resources.Load<GameObject>("Prefabs/Enemy/Enemy1");

        characterDatas.Add(character1);


    }
}

[Serializable]

public class CharacterData
{
    public int id;
    public string name;
    public int hp;
    public int level;
    public Sprite image;
    public GameObject prefab;

}
