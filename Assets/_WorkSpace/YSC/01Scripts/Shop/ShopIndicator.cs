using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopIndicator : BaseUI
{
    Image charShopButton;
    Image itemShopButton;
    Image packageShopButton;
    Image cheatShopButton;

    GameObject charShop;
    GameObject itemShop;
    GameObject packageShop;
    GameObject cheatShop;
    
    [SerializeField] Material selectedMaterial;
    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        // 버튼등록
        charShopButton = GetUI<Image>("CharShopButton");
        itemShopButton = GetUI<Image>("ItemShopButton");
        packageShopButton = GetUI<Image>("PackageShopButton");
        cheatShopButton = GetUI<Image>("CheatShopButton");

        charShop = GetUI("MarketCharacter");
        itemShop = GetUI("MarketItem");
        packageShop = GetUI("MarketPackage");
        cheatShop = GetUI("MarketCheat");
        
        LightOn(charShop);
        LightOn(itemShop);
        LightOn(packageShop);
        LightOn(cheatShop);
    }

    private void Update()
    {
        
    }
    

    public void LightOn(GameObject shop)
    {
        if (shop.activeSelf)
            Debug.Log(shop.name + " is On");
        
    }
    
}
