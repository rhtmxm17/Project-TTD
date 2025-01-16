using UnityEditor;
using UnityEngine;

public class HYJ_BtnBuff : MonoBehaviour
{
    [SerializeField] public Transform BuffPanel;
    [SerializeField] private GameObject AttackBuffImage;
    [SerializeField] private GameObject DefenseBuffImage;
    [SerializeField] private GameObject HealBuffImage;
    
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
