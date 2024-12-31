using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyItemUI : BaseUI
{
    [Header("이미지와 소지 개수를 출력할 아이템")]
    [SerializeField] ItemData item;

    [Header("하위 UI")]
    [SerializeField] RawImage rawImage;
    [SerializeField] TMP_Text numberText;

    private void Start()
    {
        rawImage.texture = item.SpriteImage.texture;
    }

    private void OnEnable()
    {
        UpdateNumber();
        item.Number.onValueChanged += UpdateNumber;
        
        // 실제 빌드 파일에서는 로딩 완료 후 UI가 생성될 것이기 때문에 필요없어질듯
        GameManager.UserData.onLoadUserDataCompleted.AddListener(UpdateNumber);
    }

    private void OnDisable()
    {
        item.Number.onValueChanged -= UpdateNumber;
        GameManager.UserData.onLoadUserDataCompleted.RemoveListener(UpdateNumber);
    }

    private void UpdateNumber(long value) => UpdateNumber();

    private void UpdateNumber()
    {
        numberText.text = item.Number.Value.ToString();
    }

}
