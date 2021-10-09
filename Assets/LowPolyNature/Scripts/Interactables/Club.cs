using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour, IDamageSource
{
    [Range(1, 100)]
    public int DamagePerHit = 20;

    public Enemy WeaponHolder;

    public int GetDamagePerHit()
    {
        return DamagePerHit;
    }

    private void OnTriggerEnter(Collider other)
    {

        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null && other is BoxCollider)
        {
            if (!player.IsDead)
            {
                WeaponHolder.CauseDamage(DamagePerHit, player);

            }
        }
    }
}
