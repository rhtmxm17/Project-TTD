using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : BaseUI
{
    OpenableUIBase mainPanel;
    OpenableUIBase characterListPanel;
    OpenableUIBase shopPanel;
    OpenableUIBase storyPanel;
    OpenableUIBase stagePanel;
    OpenableUIBase dongonPanel;
    OpenableUIBase raidPanel;

    List<OpenableUIBase> windowList;
    protected override void Awake()
    {
        base.Awake();

        mainPanel = GetUI<OpenableUIBase>("Main");

        characterListPanel = GetUI<OpenableUIBase>("Character");
        shopPanel = GetUI<OpenableUIBase>("Shop");
        storyPanel = GetUI<OpenableUIBase>("Story");

        stagePanel = GetUI<OpenableUIBase>("Stage");
        dongonPanel = GetUI<OpenableUIBase>("Dongon");
        raidPanel = GetUI<OpenableUIBase>("Raid");

        windowList = new List<OpenableUIBase>
        {
            mainPanel,
            characterListPanel,
            shopPanel,
            storyPanel,
            stagePanel,
            dongonPanel,
            raidPanel
        };

        InitButtons();
    }

    private void InitButtons()
    {
        GetUI<Button>("CharacterButton").onClick.AddListener(() => CloseOtherWindows(characterListPanel));
        GetUI<Button>("ShopButton").onClick.AddListener(() => CloseOtherWindows(shopPanel));
        GetUI<Button>("StoryButton").onClick.AddListener(() => CloseOtherWindows(storyPanel));

        GetUI<Button>("BattleButton").onClick.AddListener(() => CloseOtherWindows(stagePanel));

        GetUI<Button>("Back_ch").onClick.AddListener(() => CloseOtherWindows(mainPanel));
        GetUI<Button>("Back_sh").onClick.AddListener(() => CloseOtherWindows(mainPanel));
        GetUI<Button>("Back_st").onClick.AddListener(() => CloseOtherWindows(mainPanel));

        GetUI<Button>("Back_advSt").onClick.AddListener(() => CloseOtherWindows(mainPanel));
        GetUI<Button>("Back_advDon").onClick.AddListener(() => CloseOtherWindows(mainPanel));
        GetUI<Button>("Back_advRa").onClick.AddListener(() => CloseOtherWindows(mainPanel));

        
    }

    void CloseOtherWindows(OpenableUIBase myWindow)
    {
        foreach (OpenableUIBase window in windowList)
        {
            if (window == myWindow)
                window.OpenWindow();
            else
                window.CloseWindow();
        }
    }
    
}