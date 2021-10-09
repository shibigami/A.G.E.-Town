using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGuy : Enemy
{
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (IsDead)
        {
            GetComponent<Animator>().SetTrigger("death");
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<BoxCollider>());
        }
    }

}
