using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamageSource
{
    public float DestructionTime = 3.0f;

    [Range(1, 100)]
    public int DamagePerHit = 25;

    internal GameObject Owner = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    public bool IsPlayerOwner
    {
        get
        {
            if (Owner == null)
                return false;

            var pc = Owner.GetComponent<PlayerController>();

            return pc != null;
        }
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(DestructionTime);

        Destroy(gameObject);
    }

    public int GetDamagePerHit()
    {
        return DamagePerHit;
    }
}
