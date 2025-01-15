using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AutoButton : MonoBehaviour
{
    [SerializeField]
    AutoBattleLogic battleLogic;

    [SerializeField]
    Color inactiveColor;
    [SerializeField]
    Color activeColor;

    Button button;
    Image buttonImg;

    bool isAutoPlaying = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);

        buttonImg = GetComponent<Image>();
    }

    public void OnClicked()
    {

        if (isAutoPlaying)//오토를 끄는 동작
        {
            isAutoPlaying = false;
            buttonImg.color = inactiveColor;
            battleLogic.OffAutoBattle();
        }
        else//오토를 켜는 동작
        {
            isAutoPlaying = true;
            buttonImg.color = activeColor;
            battleLogic.OnAutoBattle();
        }
    }

}
