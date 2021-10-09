using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Berserker : Enemy
{
    public PlayerController Player;

    private NavMeshAgent mAgent;

    private Animator mAnimator;

    public float EnemyDistanceToChase = 5.0f;

    public override void OnStart()
    {
        base.OnStart();

        CanAttack = true;

        mAgent = GetComponent<NavMeshAgent>();

        mAnimator = GetComponent<Animator>();
    }

    private bool IsNavMeshMoving
    {
        get
        {
            return mAgent.velocity.magnitude > 0.1f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsDead)
            return;

        var item = other.gameObject.GetComponent<IDamageSource>();
        if (item != null)
        {
            // Hit by a damagesource
            if (Player.GetComponent<PlayerController>().IsAttacking ||
                other.gameObject.CompareTag("Bullet"))
            {
                mAnimator.SetTrigger("tr_hit");
                TakeDamage(item.GetDamagePerHit());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (IsDead || Player.IsDead)
        {
            mAnimator.SetBool("run", false);
            mAgent.enabled = false;
            return;
        }

        // Performance optimization: Thx to kyl3r123 :-)
        float squaredDist = (transform.position - Player.transform.position).sqrMagnitude;
        float EnemyDistanceChaseSqrt = EnemyDistanceToChase * EnemyDistanceToChase;
        float EnemyDistanceAttack = 1.5f * 1.5f;

        // Follow the player
        if (squaredDist < EnemyDistanceChaseSqrt)
        {
            mAgent.SetDestination(Player.transform.position);
        }

        if(squaredDist < EnemyDistanceAttack)
        {
            if (CanAttack)
            {
                mAnimator.SetTrigger("attack_1");
                CanAttack = false;
                StartCoroutine("OnCompleteAttackAnimation");

            }
        }

        mAnimator.SetBool("run", IsNavMeshMoving);
    }

    IEnumerator OnCompleteAttackAnimation()
    {
        yield return new WaitForSeconds(0.8f);

        CanAttack = true;
    }

    private bool CanAttack
    {
        get;set;
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (IsDead)
        {
            GetComponent<Animator>().SetBool("isdead", true);
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<BoxCollider>());
        }
    }

    public override void CauseDamage(int amount, IDamageable damageTo)
    {
        // Berserker is attacking and hitting the player
        if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack_1") &&
            mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
        {
             damageTo.TakeDamage(amount);
        }
    }
}
