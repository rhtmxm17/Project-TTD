using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HYJ_UnitInfo : MonoBehaviour
{
    [SerializeField] public float unitPower;
    [SerializeField] public int unitLevel;
    [SerializeField] public  ElementType unitElementType;
    [SerializeField] public RoleType unitRoleType;

    public void InitUnitInfo(CharacterData characterData)
    {
        unitLevel = characterData.Level.Value;
        unitPower = characterData.PowerLevel;
        unitElementType = characterData.StatusTable.type;
        unitRoleType = characterData.StatusTable.roleType;
    }
}
