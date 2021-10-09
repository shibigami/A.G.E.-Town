using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(InventoryItemBase item, ItemSlot slot )
    {
        Item = item;
        Slot = slot;
    }

    public InventoryItemBase Item;

    public ItemSlot Slot;

}

