using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitch : MonoBehaviour
{
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
        foreach (TabSet tab in windowList)
        {
            if (tab.tabWindow == myWindow)
                tab.tabWindow.OpenWindow();
            else
                tab.tabWindow.CloseWindow();
        }
    }
    
}