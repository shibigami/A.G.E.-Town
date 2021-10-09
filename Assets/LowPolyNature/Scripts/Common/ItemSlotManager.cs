using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotManager : MonoBehaviour
{
    protected IList<ItemSlot> mSlots = new List<ItemSlot>();

    public event System.EventHandler<InventoryEventArgs> ItemAdded;
    public event System.EventHandler<InventoryEventArgs> ItemRemoved;

    public void AddItem(InventoryItemBase item)
    {
        ItemSlot freeSlot = FindStackableSlot(item);
        if (freeSlot == null)
        {
            freeSlot = FindNextEmptySlot();
        }
        if (freeSlot != null)
        {
            freeSlot.AddItem(item);

            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item, freeSlot));
            }
        }
    }

    public IEnumerable<ItemSlot> Slots
    {
        get { return mSlots; }
    }


    public void RemoveItem(InventoryItemBase item)
    {
        foreach (ItemSlot slot in mSlots)
        {
            if (slot.Remove(item))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(this, new InventoryEventArgs(item, slot));
                }
                break;
            }
        }
    }

    protected ItemSlot FindStackableSlot(InventoryItemBase item)
    {
        foreach (ItemSlot slot in mSlots)
        {
            if (slot.IsStackable(item))
                return slot;
        }
        return null;
    }

    protected ItemSlot FindNextEmptySlot()
    {
        foreach (ItemSlot slot in mSlots)
        {
            if (slot.IsEmpty)
                return slot;
        }
        return null;
    }

    public int GetItemCount(InteractableItemBase item)
    {
        foreach (ItemSlot slot in mSlots)
        {
            if (slot.FirstItem == null)
                break;

            if (slot.FirstItem.Name == item.Name)
            {
                return slot.Count;
            }
        }
        return 0;
    }

}
