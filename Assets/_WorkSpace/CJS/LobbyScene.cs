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
        [EnumNamedArray(typeof(GameManager.MenuType))]
        public List<Button> menuSceneButtons;
    }
    [SerializeField] ChildUIField childUIField;

    private void Awake()
    {
        for (int i = 0; i < childUIField.menuSceneButtons.Count; i++)
        {
            GameManager.MenuType menu = (GameManager.MenuType)i;
            childUIField.menuSceneButtons[i].onClick.AddListener(() => GameManager.Instance.LoadMenuScene(menu));
        }
    }
}
