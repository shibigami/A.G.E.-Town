using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingPanel : MonoBehaviour, IDropHandler
{
    private Recipe mCurrentRecipe = null;

    void Start()
    {
        CraftingManager.Instance.ItemAdded += CraftingItemAdded;
        CraftingManager.Instance.ItemRemoved += CraftingItemRemoved;
    }

    private void CraftingItemRemoved(object sender, InventoryEventArgs e)
    {
        UpdateCraftingUI(e);

        UpdateCraftingResultUI();
    }

    private void CraftingItemAdded(object sender, InventoryEventArgs e)
    {
        UpdateCraftingUI(e);

        UpdateCraftingResultUI();
    }

    private void ClearCraftingResultUI()
    {
        var img = transform.Find("Result/Border/ItemImage").GetComponent<Image>();
        img.sprite = null;
        img.enabled = false;
    }

    private void UpdateCraftingResultUI()
    {
        mCurrentRecipe = CanCraft();
        bool canCraft = (mCurrentRecipe != null);

        var img = transform.Find("Result/Border/ItemImage").GetComponent<Image>();
        img.sprite = canCraft ? mCurrentRecipe.Result.Item.Image : null;
        img.enabled = canCraft;

        transform.Find("BtnCraft").GetComponent<Button>().enabled = canCraft;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Craft()
    {
        if(mCurrentRecipe != null)
        {
            // 1. Craft the result item and put it to the inventory
            for (int i = 0; i < mCurrentRecipe.Result.Count; i++)
            {
                var item = Instantiate<InventoryItemBase>(mCurrentRecipe.Result.Item);
                item.name = item.Name + "_CI_" + (i + 1);

                Inventory.Instance.AddItem(item);
                item.OnPickup();
            }

            // Remove all items from the CraftingManager
            // that were used in the inventory
            foreach(var recipeItem in mCurrentRecipe.Items)
            {
                for (int i = 0; i < recipeItem.Count; i++)
                {
                    CraftingManager.Instance.RemoveItem(recipeItem.Item);
                }
            }

            // Update the UI
            ClearCraftingResultUI();

        }
    }

    public Recipe CanCraft()
    {
        // TODO: Check all recipes in the recipe manager
        // if we can craft something
        foreach(var recipe in RecipeManager.Instance.Recipes)
        {
            bool recipeValid = false;

            foreach(var recipeItem in recipe.Items)
            {
                var invItem = recipeItem.Item;
                var count = recipeItem.Count;

                ItemSlot slot = FindSlotForRecipeItem(recipeItem);
                if(slot != null)
                {
                    recipeValid = true;
                }
                else
                {
                    recipeValid = false;
                    break;
                }
            }

            // Ok, we found a vlid recipe for crafting
            if (recipeValid) return recipe;
        }

        return null;
    }

    private ItemSlot FindSlotForRecipeItem(RecipeItem recipeItem)
    {
        ItemSlot result = null;

        foreach(var slot in CraftingManager.Instance.Slots)
        {
            if (slot.IsEmpty)
                break;

            if (slot.FirstItem.Name == recipeItem.Item.Name)
            {
                if (slot.Count >= recipeItem.Count)
                {
                    result = slot;
                    break;
                }
            }
        }

        return result;
    }

    public void Close()
    {
        // Move all items to the inventory again
        foreach(var slot in CraftingManager.Instance.Slots)
        {
            foreach(var item in slot.Items)
            { 
                Inventory.Instance.AddItem(item);
            }
        }

        CraftingManager.Instance.Clear();

        ClearUI();

        UpdateCraftingResultUI();

        gameObject.SetActive(false);
    }

    private void ClearUI()
    {
        var itemPanel = transform.Find("ItemPanel");

        foreach (Transform slotTransform in itemPanel)
        {
            Transform imageTransform = slotTransform.GetChild(0).GetChild(0);
            Transform textTransform = slotTransform.GetChild(0).GetChild(1);
            Image image = imageTransform.GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();

            image.enabled = false;
            image.sprite = null;

            txtCount.text = "";
        }
    }

    private void UpdateCraftingUI(InventoryEventArgs e)
    {
        var itemPanel = transform.Find("ItemPanel");
        int index = -1;
        foreach (Transform slotTransform in itemPanel)
        {
            index++;

            // Border... Image
            Transform imageTransform = slotTransform.GetChild(0).GetChild(0);
            Transform textTransform = slotTransform.GetChild(0).GetChild(1);
            Image image = imageTransform.GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            if (index == e.Slot.Id)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;

                int itemCount = e.Slot.Count;
                if (itemCount < 2)
                    txtCount.text = "";
                else
                    txtCount.text = itemCount.ToString();

                if(itemCount == 0)
                {
                    image.enabled = false;
                    image.sprite = null;
                }

                // Store a reference to the item
                itemDragHandler.Item = e.Slot.FirstItem;

                break;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropItemDragHandler = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>();
        var dropSlotType = dropItemDragHandler.SlotType;

        // Ok, item comes from inventory. Find a free slot
        // and add it
        if(dropSlotType == ESlotType.Inventory)
        {
            var item = dropItemDragHandler.Item;

            CraftingManager.Instance.AddItem(dropItemDragHandler.Item);

            Inventory.Instance.RemoveItem(item);
        }
    }
}
