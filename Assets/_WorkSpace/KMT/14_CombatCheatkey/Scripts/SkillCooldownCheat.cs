using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCooldownCheat : MonoBehaviour
{
    [SerializeField]
    GameObject skillButtonParent;


    public void OnClick()
    {

        var skills = skillButtonParent.GetComponentsInChildren<BasicSkillButton>();

        foreach (BasicSkillButton skill in skills)
        {
            skill.DecreseCooltime(float.MaxValue / 2);
        }

    }
}
