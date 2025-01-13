using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopIndicator : BaseUI
{
    Button marketCharacterButton;
    Button marketItemButton;
    Button marketPackageButton;
    Button marketCheatButton;

    GameObject marketChar;
    GameObject marketItem;
    GameObject marketPackage;
    GameObject marketCheat;
    
    [SerializeField] Material selectedMaterial;
    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        // 버튼등록
        marketCharacterButton = GetUI<Button>("MarketCharacterButton");
        marketCharacterButton.onClick.AddListener(() => LightOn(marketChar));
        marketItemButton = GetUI<Button>("MarketItemButton");
        marketItemButton.onClick.AddListener(() => LightOn(marketItem));
        marketPackageButton = GetUI<Button>("MarketPackageButton");
        marketPackageButton.onClick.AddListener(() => LightOn(marketPackage));
        marketCheatButton = GetUI<Button>("MarketCheatButton");
        marketCheatButton.onClick.AddListener(() => LightOn(marketCheat));

        marketChar = GetUI("MarketCharacter");
        marketItem = GetUI("MarketItem");
        marketPackage = GetUI("MarketPackage");
        marketCheat = GetUI("MarketCheat");
        
        LightOn(marketChar);
        LightOn(marketItem);
        LightOn(marketPackage);
        LightOn(marketCheat);
    }

    private void Update()
    {
        
    }
    

    public void LightOn(GameObject shop)
    {
        if (shop.activeSelf)
        {
            Debug.Log(shop.name + " is On");
            string name = $"{shop.name}Button";
            Button tab = GetUI<Button>(name);
            tab.GetComponent<Image>().color = Color.red;
        }
    }

    public void LightOff(GameObject shop)
    {
        if (!shop.activeSelf)
        {
            Debug.Log(shop.name + " is On");
            string name = $"{shop.name}Button";
            Button tab = GetUI<Button>(name);
            tab.GetComponent<Image>().color = Color.white;
        }
    }
    
}
