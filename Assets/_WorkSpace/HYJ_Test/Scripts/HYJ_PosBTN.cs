using UnityEngine;

public class HYJ_PosBTN : MonoBehaviour
{
    [SerializeField] public Transform BuffPanel;
    [SerializeField] private GameObject AttackBuffImage;
    [SerializeField] private GameObject DefenseBuffImage;
    [SerializeField] private GameObject HealBuffImage;

    public void BuffInput(bool attackBuff, bool defenseBuff, bool healBuff)
    {
        if (attackBuff)
        {
            Instantiate(AttackBuffImage, BuffPanel);
        }
        if (defenseBuff)
        {
            Instantiate(DefenseBuffImage, BuffPanel);
        }
        if (healBuff)
        {
            Instantiate(HealBuffImage, BuffPanel);
        }
    }
}
