using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : ItemSlotManager
{
    public static CraftingManager Instance = null;

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

    private CraftingManager()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new ItemSlot(i));
        }
    }

    private const int SLOTS = 9;

    public void Clear()
    {
        foreach(var slot in mSlots)
        {
            slot.Clear();
        }
    }
}
