using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem_SO", menuName ="Shop menu", order = 1)]
public class ShopItem_SO : ScriptableObject
{
    public GameObject purchasableItem;      // The actual item that the player will spawn when they purchase this item.
    public string title;
    public string description;
    public int basePurchasePrice;
    public Sprite itemImage;
}