using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : InventoryItemBase, IDamageSource
{
    [Range(1, 100)]
    public int DamagePerHit = 20;

    public int GetDamagePerHit()
    {
        return DamagePerHit;
    }

    public override void OnUse()
    {
        base.OnUse();
    }
}
