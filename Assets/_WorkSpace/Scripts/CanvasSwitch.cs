using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitch : MonoBehaviour
{
    [SerializeField] UserSettingData.PanelType panelType;
    [SerializeField] OpenableUIBase mainWindow;

    [SerializeField] List<TabSet> windowList;

    [System.Serializable]
    private struct TabSet
    {
        public Button tabButton;
        public OpenableUIBase tabWindow;
    }

    private void Awake()
    {
        InitBackButtons();

        if (panelType != UserSettingData.PanelType.NONE)
        {
            SelectWindow(windowList[UserSettingData.Instance.Data.openedPanel[(int)panelType]].tabWindow);
        }
    }

    private void InitBackButtons()
    {
        foreach (TabSet tab in windowList)
        {
            if (null != tab.tabButton)
            {
                tab.tabButton.onClick.AddListener(() => SelectWindow(tab.tabWindow));
            }

            if (null != tab.tabWindow.BackButton)
            {
                tab.tabWindow.BackButton.onClick.AddListener(() => SelectWindow(mainWindow));
            }
        }
    }

    public void SelectWindow(OpenableUIBase myWindow)
    {
        for (int i = 0; i < windowList.Count; i++)
        {
            TabSet tab = windowList[i];

            if (tab.tabWindow == myWindow)
            {
                tab.tabWindow.OpenWindow();

                if (panelType != UserSettingData.PanelType.NONE)
                {
                    // 열린 패널 번호를 저장
                    UserSettingData.Instance.Data.openedPanel[(int)panelType] = i;
                    UserSettingData.Instance.SaveSetting();
                }
            }
            else
                tab.tabWindow.CloseWindow();
        }

    }
    
}