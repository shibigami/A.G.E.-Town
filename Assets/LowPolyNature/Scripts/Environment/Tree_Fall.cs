using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Fall : MonoBehaviour
{
    private Animator mAnimator;

    public PlayerController Player;

    public GameObject HitEffect;

    public GameObject HitEffectPosition;

    public GameObject Firewood;

    public int MaxHitCount = 3;

    private int mHitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    private void Fall()
    {
        mAnimator.SetTrigger("TrFall");

        Destroy(GetComponent<BoxCollider>());

        Invoke("ShowFirewood", 4.0f);
    }

    private void ShowFirewood()
    {
        Firewood.SetActive(true);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    { 
        InteractableItemBase item = other.gameObject.GetComponent<InteractableItemBase>();
        if (item != null)
        {
            // Hit by a weapon
            if (item.ItemType == EItemType.Weapon)
            {
                if (Player.IsAttacking)
                {
                    mHitCount++;

                    var pos = HitEffectPosition.transform.position;

                    // Play hit sound
                    var hitEffect = (GameObject) Instantiate(HitEffect, pos, transform.rotation, transform.parent);
                    Destroy(hitEffect, 1.5f);

                    if (mHitCount >= MaxHitCount)
                    {
                        Fall();
                    }
                }
            }
        }
    }
}
