using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : OpenableWindow
{
    [SerializeField]
    Button helpButton;
    [SerializeField]
    Button optionButton;
    [SerializeField]
    Button toMenuButton;
    [SerializeField]
    Button retryButton;

    [SerializeField]
    Button outerAreaButton;

    float beforeScale = 1;

    private void Awake()
    {

        toMenuButton.onClick.AddListener(() => {

            Time.timeScale = 1;
            GameManager.Instance.LoadMenuScene(StageManager.Instance.PrevScene);

        });

        retryButton.onClick.AddListener(() => {

            Time.timeScale = 1;
            GameManager.Instance.LoadMenuScene(MenuType.FORMATION);

        });

        outerAreaButton.onClick.AddListener(() => {

            CloseWindow();

        });

        outerAreaButton.onClick.AddListener(CloseWindow);
    }

    public override void OpenWindow()
    {
        if (Time.timeScale == 0)
            return;

        base.OpenWindow();
        beforeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public override void CloseWindow()
    {
        Time.timeScale = beforeScale;
        base.CloseWindow();
    }


}
