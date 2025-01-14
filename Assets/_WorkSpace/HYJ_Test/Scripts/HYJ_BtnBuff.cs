using UnityEditor;
using UnityEngine;

public class HYJ_BtnBuff : MonoBehaviour
{
    [SerializeField] public Transform BuffPanel;
    [SerializeField] private GameObject AttackBuffImage;
    [SerializeField] private GameObject DefenseBuffImage;
    [SerializeField] private GameObject HealBuffImage;
    
    // TODO : 현재 버프에 해당하는 걸 타일 안에 생성하고 있는데, 차라리 그냥 타일안에 버프 3개를 자식으로 두고, 버프에 따라 활성화 하는 방식이 좋을 것 같음 
    public void BuffInput(StatusBuffType statusBuffType)
    {
        switch (statusBuffType)
        {
            case StatusBuffType.ATK_PERCENTAGE:
                Instantiate(AttackBuffImage, BuffPanel);
                break;
            case StatusBuffType.DEF_PERCENTAGE:
                Instantiate(DefenseBuffImage, BuffPanel);
                break;
        }
    }
}
