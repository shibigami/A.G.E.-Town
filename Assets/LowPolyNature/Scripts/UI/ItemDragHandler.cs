using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ESlotType
{
    Inventory,
    Crafting
}

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItemBase Item { get; set; }

    public ESlotType SlotType = ESlotType.Inventory;

    private GameObject mDragImage;

    private RectTransform mDragSurface;


    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        mDragImage = new GameObject("Drag_Image");

        mDragImage.transform.SetParent(canvas.transform, false);
        mDragImage.transform.SetAsLastSibling();
        
        var image = mDragImage.AddComponent<Image>();

        image.sprite = GetComponent<Image>().sprite;

        image.raycastTarget = false;

        mDragSurface = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        var rt = mDragImage.GetComponent<RectTransform>();
        Vector3 mousePosition;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mDragSurface,
            data.position, data.pressEventCamera, out mousePosition))
        {
            rt.position = mousePosition;
            rt.rotation = mDragSurface.rotation;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (mDragImage != null)
            SetDraggedPosition(data);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (mDragImage != null)
        {
            Destroy(mDragImage);
        }
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
