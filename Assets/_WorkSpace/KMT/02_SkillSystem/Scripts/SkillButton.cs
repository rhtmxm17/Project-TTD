using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{

    [SerializeField]
    protected Image cooldownImg;
    [SerializeField]
    protected Button skillButton;

    public bool Interactable => skillButton.interactable;


}
