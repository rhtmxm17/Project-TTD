using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class MonsterInfoDisplayer : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField]
    GameObject monsterWaveParent;

    CombManager[] managers;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        managers = monsterWaveParent.GetComponentsInChildren<CombManager>(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(managers.Length == 0)
            managers = monsterWaveParent.GetComponentsInChildren<CombManager>(true);


        Combatable[] monsters = null;
        foreach (var manager in managers)
        {
            monsters = manager.GetComponentsInChildren<Combatable>(true);
            if (monsters.Length > 0)
                break;
        }

        if (monsters == null)
            return;

        StringBuilder sb = new StringBuilder();

        foreach (Combatable m in monsters)
        {
            CharacterData characterData = m.characterData;

            sb.Append("name : ")
                .Append(characterData.Name)
                .AppendLine()
                .Append("HP : ")
                .Append(m.Hp)
                .Append(" / ")
                .Append(m.MaxHp)
                .AppendLine()
                .Append("Def : ")
                .Append(m.Defense)
                .AppendLine()
                .Append("DefConst : ")
                .Append(m.defConst)
                .AppendLine()
                .Append("==================")
                .AppendLine();
        }

        text.text = sb.ToString();

    }
}
