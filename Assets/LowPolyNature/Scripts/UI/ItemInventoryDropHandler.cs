using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInventoryDropHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var dropItemDragHandler = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>();
        var dropSlotType = dropItemDragHandler.SlotType;

        // Ok, item comes from crafting panel. Find a free slot
        // and add it
        if (dropSlotType == ESlotType.Crafting)
        {
            var item = dropItemDragHandler.Item;

            Inventory.Instance.AddItem(dropItemDragHandler.Item);

            CraftingManager.Instance.RemoveItem(item);
        }

        //RectTransform invPanel = transform as RectTransform;

        //if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel,
        //    Input.mousePosition))
        //{

        //    InventoryItemBase item = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>().Item;
        //    if(item != null)
        //    {
        //        //_Inventory.RemoveItem(item);
        //        //item.OnDrop();
        //    }

        //}
    }
}
