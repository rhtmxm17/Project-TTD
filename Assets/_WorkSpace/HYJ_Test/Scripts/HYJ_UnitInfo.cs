using UnityEngine;

public class HYJ_UnitInfo : MonoBehaviour
{
    [SerializeField] public float unitPower;
    [SerializeField] public int unitLevel;
    [SerializeField] public  ElementType unitElementType;
    [SerializeField] public RoleType unitRoleType;

    /// <summary>
    /// 필터/정렬 리스트용 유닛정보
    /// </summary>
    /// <param name="characterData"></param>
    public void InitUnitInfo(CharacterData characterData)
    {
        unitLevel = characterData.Level.Value;
        unitPower = characterData.PowerLevel;
        unitElementType = characterData.StatusTable.type;//속성
        unitRoleType = characterData.StatusTable.roleType;//역할군
    }
}
