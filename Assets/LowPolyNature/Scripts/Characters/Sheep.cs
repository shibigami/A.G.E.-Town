using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    private NavMeshAgent mAgent;

    private Animator mAnimator;

    public GameObject Player;

    public float EnemyDistanceRun = 4.0f;

    private bool mIsDead = false;

    public GameObject[] ItemsDeadState = null;

    // Use this for initialization
    void Start()
    {
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

    void OnCollisionEnter(Collision collision)
    {

        var item = collision.collider.gameObject.GetComponent<IDamageSource>();
        if(item != null && !mIsDead)
        {
            var interactItem = item as InteractableItemBase;

            // Hit by a weapon
            if(interactItem != null && interactItem.ItemType == EItemType.Weapon)
            {
                if(Player.GetComponent<PlayerController>().IsAttacking)
                {
                    SheepDead();
                }
            }
            else 
            {
                SheepDead();
            }
        }

    }

    private void SheepDead()
    {
        mIsDead = true;
        mAgent.enabled = false;
        mAnimator.SetTrigger("die");
        Destroy(GetComponent<Rigidbody>());

        Invoke("ShowItemsDeadState", 1.2f);
    }

    void ShowItemsDeadState()
    {
        // Activate the items
        foreach(var item in ItemsDeadState)
        {
            item.SetActive(true);
        }

        Destroy(GetComponent<CapsuleCollider>());

        // Hide the sheep mesh
        transform.Find("sheep_mesh").GetComponent<SkinnedMeshRenderer>().enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (mIsDead)
            return;

        // Only runaway if player is armed
        bool isPlayerArmed = Player.GetComponent<PlayerController>().IsArmed;

        // Performance optimization: Thx to kyl3r123 :-)
        float squaredDist = (transform.position - Player.transform.position).sqrMagnitude;
        float EnemyDistanceRunSqrt = EnemyDistanceRun * EnemyDistanceRun;

        // Run away from player
        if (squaredDist < EnemyDistanceRunSqrt && isPlayerArmed)
        {
            // Vector player to me
            Vector3 dirToPlayer = transform.position - Player.transform.position;

            Vector3 newPos = transform.position + dirToPlayer;

            mAgent.SetDestination(newPos);

        }

        mAnimator.SetBool("walk", IsNavMeshMoving);

    }
}
