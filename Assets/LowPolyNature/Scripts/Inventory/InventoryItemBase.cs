using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Default,
    Consumable,
    Weapon
}

public class InteractableItemBase : MonoBehaviour
{
    public string Name;

    public Sprite Image;

    public string InteractText = "Press F to pickup the item";

    public EItemType ItemType;

    public virtual void OnInteractAnimation(Animator animator)
    {
        animator.SetTrigger("tr_pickup");
    }

    public virtual void OnAction(Animator animator, RaycastHit hit)
    {
        animator.SetTrigger("attack_1");
    }

    public virtual void OnInteract()
    {
    }

    public virtual bool CanInteract(Collider other)
    {
        return true;   
    }

    public virtual bool ContinueInteract()
    {
        return false;
    }
}

public class InventoryItemBase : InteractableItemBase
{
    public ItemSlot Slot
    {
        get; set;
    }

    public string ActionTrigger;

    public virtual void OnUse()
    {
        transform.localPosition = PickPosition;
        transform.localEulerAngles = PickRotation;
    }

    public virtual void OnDrop()
    {
        // Drop item from inventory to scene
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000))
        {
            gameObject.SetActive(true);
            var collider = gameObject.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            gameObject.transform.position = hit.point;
            gameObject.transform.eulerAngles = DropRotation;
        }
    }

    public virtual void OnPickup()
    {
        Destroy(gameObject.GetComponent<Rigidbody>());
        var collider = gameObject.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        gameObject.SetActive(false); 
    }

    public Vector3 PickPosition;

    public Vector3 PickRotation;

    public Vector3 DropRotation;

    public bool UseItemAfterPickup = false;
}
