using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemShopManager : MonoBehaviour
{
    public PlayerMoney playerMoney;                 // A reference to the script that controls the money of the player.
    public TMP_Text playerBalance;                  // The text object on the shop that shows the player how much money they have got.
    public bool menuOnScreen;                       // Bool value which keeps track of whether or not this menu is currently visible on screen.
    public GameObject itemShop;                     // The item shop GameObject
    public GameObject gameUI;                       // Default HUD for the game
    public ShopItem_SO[] weaponItems;               // The scriptable objects that contain the information about the items we are showing in the shop.
    public ShopItemTemplate[] itemPanels;           // The panels where we will display the items in the shop.
    public Button[] purchaseButtons;                // A list of all of the "Purchase / Buy" button that exists on each panel.
    
    private void Start()
    {
        ViewItemShop(false);
    }

    private void OnValidate()
    {
        ViewItemShop(menuOnScreen);
    }

    public void ViewItemShop(bool visible)
    {
        UpdateBalance();
        LoadWeaponPanels(); // Load all of the panels with data and enable them so they can be seen.
        itemShop.SetActive(visible);
        gameUI.SetActive(!visible);
        menuOnScreen = visible;

        if (visible)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = visible;
        menuOnScreen = visible;
    }

    void UpdateBalance()
    {
        int balance = playerMoney.GetSavedBalance();
        playerBalance.text = balance.ToString();
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
        CheckPurchasable();
    }

    void CheckPurchasable() // A function to enable / disable the "Purchase" button on each item panel depending if the player can afford the item or not.
    {
        UpdateBalance();

        for (int i = 0; i < weaponItems.Length; i++)
        {
            bool purchasable = playerMoney.currentBalance >= weaponItems[i].basePurchasePrice;
            print("Checking if player can afford " + weaponItems[i].name + " which has a cost of " + weaponItems[i].basePurchasePrice + ". Player has " + playerMoney.currentBalance + " coins, purchasable = " + purchasable);
            purchaseButtons[i].interactable = purchasable; // Set "Interactable" to T/F depending if the player has got enough money to buy the item.
        }
    }
}