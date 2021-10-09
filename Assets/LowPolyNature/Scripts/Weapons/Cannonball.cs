using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : InventoryItemBase, IDamageSource
{
    private bool mIsFlying = false;

    public bool IsFlying
    {
        get
        {
            return mIsFlying;
        }

        set
        {
            mIsFlying = value;
        }
    }

    [Range(1, 100)]
    public int DamagePerHit = 20;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var collisionScript in 
            collision.gameObject.GetComponents<MonoBehaviour>())
        {
            if (collisionScript is IDamageable)
            {
                (collisionScript as IDamageable).TakeDamage(DamagePerHit);
            }
        }
    }

    public int GetDamagePerHit()
    {
        return DamagePerHit;
    }
}
