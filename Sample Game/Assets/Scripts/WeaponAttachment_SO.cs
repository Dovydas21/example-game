using System.Collections;
using System.Collections.Generic;
using AttachmentTypes;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponAttachment_SO", menuName = "New weapon attachment", order = 1)]
public class WeaponAttachment_SO : ScriptableObject
{
    public attachmentTypes attachmentType;
    public GameObject attachmentPrefab;      // The actual item that the player will spawn when they purchase this item.
    public string title;
    public Sprite itemImage;

    /*

    Probably need to add some logic here that will effect the stats of the gun that this attachment is equipped to.
     
     */
}

namespace AttachmentTypes {
    public enum attachmentTypes { Sight, Grip, Barrel, Magazene }
}