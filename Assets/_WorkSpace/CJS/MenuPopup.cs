using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPopup : MonoBehaviour
{
    [SerializeField] Button backGroundButton;
    [SerializeField] Button titleButton;
    [SerializeField] Toggle cheatToggle;

    private void Awake()
    {
        backGroundButton.onClick.AddListener(() => this.gameObject.SetActive(false));
        cheatToggle.onValueChanged.AddListener(value => GameManager.Instance.IsCheatMode = value);
        titleButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
            SceneManager.LoadScene("LoginScene");
        });
    }
}
