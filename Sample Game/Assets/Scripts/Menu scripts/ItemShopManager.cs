using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopManager : MonoBehaviour
{
    public ShopItem_SO[] weaponItems;               // The scriptable objects that contain the information about the items we are showing in the shop.
    public ShopItemTemplate[] itemPanels;           // The panels where we will display the items in the shop.
    public PlayerMoney playerMoney;                 // A reference to the script that controls the money of the player.

    private void Start()
    {
        LoadWeaponPanels(); // Load all of the panels with data and enable them so they can be seen.
    }

    void LoadWeaponPanels()
    {
        // Enable as many panels as we have items to show.
        for (int i = 0; i < weaponItems.Length; i++)
        {
            itemPanels[i].gameObject.SetActive(true);
        }

        // Make each panel contain the correct information.
        for (int i = 0; i < weaponItems.Length; i++)
        {
            itemPanels[i].descriptionText.text = weaponItems[i].description;
            itemPanels[i].titleText.text = weaponItems[i].title;
            itemPanels[i].basePurchasePrice.text = weaponItems[i].basePurchasePrice.ToString() + " coins";
            itemPanels[i].itemImage.sprite = weaponItems[i].itemImage;
            itemPanels[i].itemImage.preserveAspect = true;
        }
    }
}
