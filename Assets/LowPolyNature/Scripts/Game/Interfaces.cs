using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageSource
{
    int GetDamagePerHit();
}

public interface IDamageable
{
    void CauseDamage(int amount, IDamageable damageTo);

    void TakeDamage(int amount);
}


public class Enemy : MonoBehaviour, IDamageable
{
    public event EventHandler Died;

    private bool mIsDead = false;

    public int Health = 100;

    private NPC_Healthbar mHealthBar;

    public void Start()
    {
        IsDead = false;

        var tfHealthBar = transform.Find("Healthbar");
        if(tfHealthBar != null)
        {
            mHealthBar = tfHealthBar.GetComponent<NPC_Healthbar>();

        }

        OnStart();
    }

    public virtual void OnStart()
    {
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= amount;

        if(Health <= 0)
        {
            Health = 0;

            IsDead = true;
        }

        if(mHealthBar != null)
        {
            mHealthBar.SetHealth(Health);

            if(IsDead)
            {
                mHealthBar.gameObject.SetActive(false);
            }
        }
    }

    public virtual void CauseDamage(int amount, IDamageable damageTo)
    {
    }

    public virtual bool IsDead
    {
        get { return mIsDead; }
        protected set
        {
            if(mIsDead != value)
            {
                mIsDead = value;

                if(mIsDead)
                {
                    if(Died != null)
                    {
                        Died(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}
