using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    private Transform mItemImage;

    public void OnDrop(PointerEventData eventData)
    {
        mItemImage = transform.Find("Border/ItemImage");

        var image = mItemImage.GetComponent<Image>();

        var itemDragHandler = mItemImage.GetComponent<ItemDragHandler>();
        var dropItemDragHandler = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>();

        var slotType = itemDragHandler.SlotType;
        var dropSlotType = dropItemDragHandler.SlotType;

        // Depending on the slot types we have to execute different
        // Remove and add logic for inventory and crafting panel.

        // => We dont allow (ATM) moving in the same panel (next version)

        // 1. Move from inventory to Crafting panel
        if (slotType == ESlotType.Crafting && dropSlotType == ESlotType.Inventory)
        {
            itemDragHandler.Item = dropItemDragHandler.Item;

            // Assign new image and enable it
            image.enabled = true;
            image.sprite = itemDragHandler.Item.Image;

            // Remove the item from the inventory
            // Make sure that the player doesnt carry an item
            // or that it isnt dropped when removing
            Inventory.Instance.RemoveItem(itemDragHandler.Item);
        }

        // 2. Move from Carfting to inventory
        if (slotType == ESlotType.Inventory && dropSlotType == ESlotType.Crafting)
        {

        }
    }

}
