using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLists : BaseUI
{

    List<SkillButtonGroup> groupList = new List<SkillButtonGroup>();

    SkillButtonGroup levelupGroup;
    SkillButtonGroup secondSkillGroup;

    protected override void Awake()
    {
        base.Awake();
        levelupGroup = GetUI<SkillButtonGroup>("LevelupGuideLine");
        secondSkillGroup = GetUI<SkillButtonGroup>("SecondSkillGuideLine");

        levelupGroup.hideAllGroups += HideAll;
        secondSkillGroup.hideAllGroups += HideAll;

        groupList.Add(levelupGroup);
        groupList.Add(secondSkillGroup);
    }

    void HideAll()
    {
        foreach (SkillButtonGroup group in groupList)
        {
            group.HideButtonGroup();
        }
    }

}
