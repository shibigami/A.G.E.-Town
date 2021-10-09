using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : ItemSlotManager
{
    private const int SLOTS = 9;

    public static Inventory Instance = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Dont destroy on reloading the scene
        // DontDestroyOnLoad(gameObject);
    }

    public event EventHandler<InventoryEventArgs> ItemUsed;

    public Inventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new ItemSlot(i));
        }
    }



    public void UseItem(InventoryItemBase item)
    {
        if (ItemUsed != null)
        {
            ItemUsed(this, new InventoryEventArgs(item, item.Slot));
        }

        item.OnUse();
    }


}
