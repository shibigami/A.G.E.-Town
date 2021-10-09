using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crate : InteractableItemBase
{

    private bool mIsOpen = false;

    public bool OpenOnly = false;

    public UnityEvent OnOpened;

    public override bool CanInteract(Collider other)
    {
        return !(OpenOnly && mIsOpen);
    }

    public override bool ContinueInteract()
    {
        return CanInteract(null);
    }

    public override void OnInteract()
    {
        string open = "open";
        string close = "close";

        InteractText = "Press F to ";

        if (OpenOnly)
        {
            InteractText += open;
        }
        else
        {
            InteractText += mIsOpen ? open : close;
        }

        mIsOpen = !mIsOpen;

        GetComponent<Animator>().SetBool("open", mIsOpen);

        if(mIsOpen)
        {
            if(OnOpened != null)
            {
                OnOpened.Invoke();
            }
        }
    }
}
