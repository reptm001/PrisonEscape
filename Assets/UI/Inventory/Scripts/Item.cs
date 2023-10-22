using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        BlueKey,
        GreenKey,
        GreyKey,
        OrangeKey,
        PinkKey,
        PurpleKey,
        RedKey,
        YellowKey,
        Rock,
        NullKey,
    }

    public ItemType itemType;
    public int amount;
}

