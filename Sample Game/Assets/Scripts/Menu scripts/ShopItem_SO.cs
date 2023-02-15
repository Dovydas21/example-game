using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem_SO", menuName ="Shop menu", order = 1)]
public class ShopItem_SO : ScriptableObject
{
    public string title;
    public string description;
    public float basePurchasePrice;
    public Sprite itemImage;
}
