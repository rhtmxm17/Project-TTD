using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendBaseUI : BaseUI
{

    public enum Tabs { FRIENDS, SEARCH, REQUESTED, RECIEVED}

    Dictionary<Tabs, (FocusableTab, OpenableWindow)> tabDictionary = new Dictionary<Tabs, (FocusableTab, OpenableWindow)>();
    
    protected override void Awake()
    {
        base.Awake();

        GetUI<Button>("FriendTab").onClick.AddListener(() => SelectTab(Tabs.FRIENDS));
        GetUI<Button>("SearchTab").onClick.AddListener(() => SelectTab(Tabs.SEARCH));
        GetUI<Button>("RequestedTab").onClick.AddListener(() => SelectTab(Tabs.REQUESTED));
        GetUI<Button>("RecievedTab").onClick.AddListener(() => SelectTab(Tabs.RECIEVED));

        // GetUI<Button>("BackgroundPadder").onClick.AddListener(() => GetComponent<OpenableWindow>().CloseWindow());

        tabDictionary.Add(Tabs.FRIENDS, (GetUI<FocusableTab>("FriendTab"), GetUI<OpenableWindow>("FriendWindow")));
        tabDictionary.Add(Tabs.SEARCH, (GetUI<FocusableTab>("SearchTab"), GetUI<OpenableWindow>("SearchWindow")));
        tabDictionary.Add(Tabs.REQUESTED, (GetUI<FocusableTab>("RequestedTab"), GetUI<OpenableWindow>("RequestedWindow")));
        tabDictionary.Add(Tabs.RECIEVED, (GetUI<FocusableTab>("RecievedTab"), GetUI<OpenableWindow>("RecievedWindow")));

        // SHW: 오픈윈도우 삭제 후 널래퍼런스 오류로 인해 주석처리 합니다.
        // GetComponent<OpenableWindow>().onOpenAction += () => SelectTab(Tabs.FRIENDS);
        // GetComponent<OpenableWindow>().onCloseAction += () => CloseAllTab();

        // GetComponent<OpenableWindow>().CloseWindow();
    }

    public void SelectTab(Tabs tapType)
    {
        if (tabDictionary[tapType].Item2.gameObject.activeSelf)
        {
            return;
        }
        
        CloseAllTab();

        tabDictionary[tapType].Item1.Focus();
        tabDictionary[tapType].Item2.OpenWindow();
    }

    public void CloseAllTab()
    {
        foreach (var tab in tabDictionary.Values)
        {
            tab.Item1.Relese();
            tab.Item2.CloseWindow();
        }
    }

}
