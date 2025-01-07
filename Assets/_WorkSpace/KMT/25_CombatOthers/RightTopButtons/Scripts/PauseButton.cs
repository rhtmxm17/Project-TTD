using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        if (Time.timeScale == 0)
            return;

        float beforeScale = Time.timeScale;
        Time.timeScale = 0;

        GameManager.OverlayUIManager.OpenSimpleInfoPopup(
                "더 월드!",
                "재재생",
                () => Time.timeScale = beforeScale
        );
    }
}
