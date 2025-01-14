using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HYJ_UnitInfo : MonoBehaviour
{
    [SerializeField] private float unitPower;
    [SerializeField] private int unitLevel;
    [SerializeField] private  ElementType unitElementType;
    [SerializeField] private RoleType unitRoleType;

    public void InitUnitInfo(CharacterData characterData)
    {
        unitLevel = characterData.Level.Value;
        unitPower = characterData.PowerLevel;
        unitElementType = characterData.StatusTable.type;
        unitRoleType = characterData.StatusTable.roleType;
    }
}
