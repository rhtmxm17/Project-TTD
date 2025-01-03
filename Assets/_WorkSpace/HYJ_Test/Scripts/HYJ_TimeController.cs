using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HYJ_TimeController : MonoBehaviour
{
    bool IsPause;
    float gameTime;
    [SerializeField] TextMeshProUGUI speedChangeBTN;

    private void Start()
    {
        IsPause = false;
        gameTime = 1;
    }

    public void PauseGame()
    {
        if (!IsPause)
        {
            IsPause = true;
            Time.timeScale = 0;
        }

        else if (IsPause)
        {
            IsPause = false;
            Time.timeScale = gameTime;
        }
    }

    public void SpeedChange()
    {
        switch (gameTime)
        {
            case 1:
                gameTime = 1.5f;
                speedChangeBTN.text = "x1.5";
                Time.timeScale = gameTime;
                break;
            case 1.5f:
                gameTime = 2;
                speedChangeBTN.text = "x2";
                Time.timeScale = gameTime;
                break;
            case 2:
                gameTime = 1;
                speedChangeBTN.text = "x1";
                Time.timeScale = gameTime;
                break;
        }
    }
}
