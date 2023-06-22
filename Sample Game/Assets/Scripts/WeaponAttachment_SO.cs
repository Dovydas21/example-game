using System.Collections;
using System.Collections.Generic;
using AttachmentTypes;
using UnityEngine;

namespace AttachmentTypes       // Declared enum in a namespace so it can be referred to in other classes. (using AttachmentTypes;)
{
    public enum attachmentTypes { Sight, Grip, Barrel, Magazene }
}


[CreateAssetMenu(fileName = "WeaponAttachment_SO", menuName = "New weapon attachment", order = 1)]
public class WeaponAttachment_SO : ScriptableObject
{
    public attachmentTypes attachmentType;
    public GameObject attachmentPrefab;                 // The actual item that the player will spawn when they purchase this item.
    public string title;                                // The title / name of the attachment.
    public Sprite itemImage;                            // An image of the item.

    [Header("Sight settings")]
    public Vector3 aimPosition = Vector3.zero;          // Only required if the attachment type is a Sight
    public string reticleObjectName;                    // The name of the object that is where the reticle is. Only required if the attachment type is a Sight

    /*

    Probably need to add some logic here that will effect the stats of the gun that this attachment is equipped to.

     */
}
