using System;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UTalk;

public enum EnUIMode
{
    CraftingUI,
    RecipeBookUI,
    None
}

public class HUD : MonoBehaviour
{
    public Inventory Inventory;

    public bool ShowMiniMap = true;

    private GameObject mMessagePanel;

    private RectTransform mBarsPanel;

    private Transform mWeaponImg;

    private Text mCurrentAmmo;

    private EnUIMode mUIMode = EnUIMode.None;

    // Use this for initialization
    void Start()
    {
        var miniMap = transform.Find("Minimap");

        mBarsPanel = transform.Find("Bars_Panel").GetComponent<RectTransform>();

        mWeaponImg = transform.Find("Bars_Panel/Weapon_Img");

        mCurrentAmmo = transform.Find("Bars_Panel/CurrentAmmo").GetComponent<Text>();

        mMessagePanel = transform.Find("MessagePanel").gameObject;

        miniMap.gameObject.SetActive(ShowMiniMap);

        Inventory.ItemAdded += InventoryScript_ItemAdded;
        Inventory.ItemRemoved += Inventory_ItemRemoved;
        Inventory.ItemUsed += Inventory_ItemUsed;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.C))
            {
                OpenUI(EnUIMode.CraftingUI);
            }
        }

    }

    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        var gun = e.Item.GetComponent<Gun>();

        bool showGun = (gun != null);

        SetShowWeaponUI(showGun);

        if (showGun)
            mCurrentAmmo.text = gun.Ammo.ToString();
    }

    public void UpdateUI(PlayerController player)
    {
        var item = player.GetCurrentItem();
        if (item != null)
        {
            var gun = item.GetComponent<Gun>();
            mCurrentAmmo.text = gun.Ammo.ToString();
        }
    }

    private void SetShowWeaponUI(bool showGun)
    {
        int height = (showGun) ? 200 : 140;

        SetBarsPanelHeight(height);

        mCurrentAmmo.gameObject.SetActive(showGun);

        mWeaponImg.gameObject.SetActive(showGun);
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        UpdateInventoryUI(e);
    }

    private void UpdateInventoryUI(InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        int index = -1;
        foreach (Transform slot in inventoryPanel)
        {
            index++;

            // Border... Image
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform textTransform = slot.GetChild(0).GetChild(1);
            Image image = imageTransform.GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            if (index == e.Slot.Id)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;

                int itemCount = e.Slot.Count;
                if (itemCount < 2)
                {
                    txtCount.text = "";
                }
                else
                {
                    txtCount.text = itemCount.ToString();
                }

                if (itemCount == 0)
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

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        var gun = e.Item.GetComponent<Gun>();

        if (gun != null)
            SetShowWeaponUI(false);

        UpdateInventoryUI(e);
    }

    private bool mIsMessagePanelOpened = false;

    public bool IsMessagePanelOpened
    {
        get { return mIsMessagePanelOpened; }
    }

    public void OpenMessagePanel(InteractableItemBase item)
    {
        mMessagePanel.SetActive(true);

        Text mpText = mMessagePanel.transform.Find("Text").GetComponent<Text>();
        mpText.text = item.InteractText;

        mIsMessagePanelOpened = true;
    }

    public void OpenMessagePanel(string text)
    {
        mMessagePanel.SetActive(true);

        Text mpText = mMessagePanel.transform.Find("Text").GetComponent<Text>();
        mpText.text = text;

        mIsMessagePanelOpened = true;
    }

    public void CloseMessagePanel()
    {
        mMessagePanel.SetActive(false);

        mIsMessagePanelOpened = false;
    }

    private void OpenCraftingUI()
    {
        var craftingUI = transform.Find("CraftingPanel");
        if (craftingUI != null)
        {
            craftingUI.GetComponent<CraftingPanel>().Open();
        }
    }

    private void OpenUI(EnUIMode uiMode)
    {
        if (mUIMode != EnUIMode.None || ConversationManager.Instance.IsConversationActive)
            return;

        switch(uiMode)
        {
            case EnUIMode.CraftingUI:
                OpenCraftingUI();
                break;
            case EnUIMode.RecipeBookUI:
                OpenRecipeBookUI();
                break;
        }

        mUIMode = uiMode;


    }

    public void CloseCurrentUI()
    {
        if (mUIMode == EnUIMode.None)
            return;

        switch(mUIMode)
        {
            case EnUIMode.CraftingUI:
                CloseCraftingUI();
                break;
            case EnUIMode.RecipeBookUI:
                CloseRecipeBookUI();
                break;
        }

        mUIMode = EnUIMode.None;

    }

    private void CloseRecipeBookUI()
    {

    }

    private void CloseCraftingUI()
    {
        var craftingUI = transform.Find("CraftingPanel");
        if (craftingUI != null)
        {
            craftingUI.GetComponent<CraftingPanel>().Close();
        }
    }

    private void OpenRecipeBookUI()
    {
    }

    public void SetBarsPanelHeight(int height)
        {
            mBarsPanel.sizeDelta = new Vector2(mBarsPanel.rect.width, height);
        }
    }
