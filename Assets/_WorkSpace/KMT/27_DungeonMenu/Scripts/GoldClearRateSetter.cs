using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

class Numbers {

    public static string ZERO = "0";
    public static string ONE = "1";
    public static string TWO = "2";
    public static string THREE = "3";
    public static string FOUR = "4";
    public static string FIVE = "5";

    public static string IntToStr(int val)
    {

        switch (val)
        {
            case 0: return ZERO;
            case 1: return ONE;
            case 2: return TWO;
            case 3: return THREE;
            case 4: return FOUR;
            case 5: return FIVE;
            default:
                return "";
        }

    }
}

public class GoldClearRateSetter : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    Button enterButton;
    [SerializeField]
    Button skipButton;

    [Header("Dungeon Type")]
    [SerializeField]
    StageType stageType;

    Slider slider;
    Dictionary<string, long> dungeonClearRateDic;


    private void Awake()
    {
        switch (stageType)
        {
            case StageType.GOLD:
                dungeonClearRateDic = GameManager.UserData.PlayData.GoldDungeonClearRate.Value;
                break;
            case StageType.EXP:
                dungeonClearRateDic = GameManager.UserData.PlayData.ExpDungeonClearRate.Value;
                break;
            case StageType.ENFORCE:
            dungeonClearRateDic = GameManager.UserData.PlayData.EnforceDungeonClearRate.Value;
                break;
            default:
                break;
        }

        slider = GetComponent<Slider>();

        if (dungeonClearRateDic.Count == 0)
        {
            slider.value = 0;
            enterButton.interactable = true;
            skipButton.interactable = false;
            text.text = $"0%";
        }
        else if (dungeonClearRateDic.ContainsKey(Numbers.ZERO))
        {
            long clearRate = dungeonClearRateDic[Numbers.ZERO];

            enterButton.interactable = true;

            if (clearRate >= 100)
                skipButton.interactable = true;
            else
                skipButton.interactable = false;

            slider.value = clearRate / 100f;
            text.text = $"{clearRate}%";
        }
        else
        {
            //AFTER : 예외사항 또는 다른 로직 발생시 사용
            Debug.Log("예외로직 진입");
        }

    }

    public void OnSliderChanged(int idx)
    {

        if (idx == 0)
        {
            enterButton.interactable = true;

            if (dungeonClearRateDic.ContainsKey(Numbers.ZERO))
            {
                long clearRate = dungeonClearRateDic[Numbers.ZERO];

                if (clearRate >= 100)
                    skipButton.interactable = true;
                else
                    skipButton.interactable = false;

                slider.value = clearRate / 100f;
                text.text = $"{clearRate}%";
            }
            else
            {
                slider.value = 0;
                skipButton.interactable = false;
                text.text = $"0%";
            }
        }
        else if (dungeonClearRateDic.ContainsKey(Numbers.IntToStr(idx)))
        {
            long clearRate = dungeonClearRateDic[Numbers.IntToStr(idx)];

            if (!dungeonClearRateDic.ContainsKey(Numbers.IntToStr(idx - 1)))
            {
                enterButton.interactable = false;
            }
            else
            {
                if(dungeonClearRateDic[Numbers.IntToStr(idx - 1)] >= 100)
                    enterButton.interactable = true;
                else
                    enterButton.interactable = false;
            }

            if (clearRate >= 100)
                skipButton.interactable = true;
            else
                skipButton.interactable = false;

            slider.value = clearRate / 100f;
            text.text = $"{clearRate}%";
        }
        else
        {
            if (!dungeonClearRateDic.ContainsKey(Numbers.IntToStr(idx - 1)))
            {
                enterButton.interactable = false;
            }
            else
            {
                if (dungeonClearRateDic[Numbers.IntToStr(idx - 1)] >= 100)
                    enterButton.interactable = true;
                else
                    enterButton.interactable = false;
            }

            slider.value = 0;
            skipButton.interactable = false;
            text.text = $"0%";
        }

    }



}
