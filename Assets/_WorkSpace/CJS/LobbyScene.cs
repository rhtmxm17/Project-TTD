using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    [System.Serializable]
    private struct ChildUIField
    {
        public Button charactersButton;
        public Button achievementButton;
        public Button storyButton;
        public Button shopButton;
        public Button myRoomButton;
        public Button advantureButton;
    }
    [SerializeField] ChildUIField childUIField;

    private void Awake()
    {

    }
}
