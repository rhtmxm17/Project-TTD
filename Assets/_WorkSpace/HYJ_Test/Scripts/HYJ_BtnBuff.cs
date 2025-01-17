using UnityEngine;
using UnityEngine.UI;

public class HYJ_BtnBuff : MonoBehaviour
{
    // FixMe : 현재 버프에 해당하는 걸 타일 안에 생성하고 있는데, 타일안에 버프 3개를 자식으로 두고, 버프에 따라 활성화 하는 방식이 좋을 것 같음
    [SerializeField] public Transform BuffPanel; // 배치 버튼 내부의 버프 패널
    [SerializeField] private GameObject AttackBuffImage;
    [SerializeField] private GameObject DefenseBuffImage;
    [SerializeField] private GameObject HealBuffImage;
    
    [Header("버프효과 이펙트 메테리얼")]                           
    [SerializeField] Material atkBuff;  // 공격버프, 외각강조:빨간 
    [SerializeField] Material defBuff;  // 방어버프, 외각강조:파란 
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="statusBuffType"> </param>
    public void BuffInput(StatusBuffType statusBuffType)
    {
        var parentObj = BuffPanel.parent.gameObject;
        switch (statusBuffType)
        {
            case StatusBuffType.ATK_PERCENTAGE:
                Instantiate(AttackBuffImage, BuffPanel);
                parentObj.GetComponent<Image>().material = atkBuff; // 부모 이미지 컴포넌트 메테리얼변경
                break;
            case StatusBuffType.DEF_PERCENTAGE:
                Instantiate(DefenseBuffImage, BuffPanel);
                parentObj.GetComponent<Image>().material = defBuff; // 부모 이미지 컴포넌트 메테리얼변경
                break;
        }
    }
}
