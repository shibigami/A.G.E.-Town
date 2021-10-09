using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : InventoryItemBase
{
    public Transform BulletPrefab;

    private Transform mBulletSpawnPos;

    public float ShootForce = 100.0f;

    public int DamagePerHit = 25;

    private RaycastHit mHit;

    public AudioClip ShotAudio;

    private AudioSource mAudioSource;

    public int Ammo = 10;

    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();

        mBulletSpawnPos = transform.Find("Spawnpos");

        mAudioSource.clip = ShotAudio;
    }

    public override void OnAction(Animator animator, RaycastHit hit)
    {
        mHit = hit;
        animator.SetTrigger("tr_pistol_fire");
    }

    public void FireBullet()
    {
        if (Ammo > 0)
        {
            Ammo--;

            mAudioSource.Play();

            var rot = Quaternion.LookRotation(mHit.point - mBulletSpawnPos.position);

            mBulletSpawnPos.rotation = rot;

            var bullet = (Transform)Instantiate(BulletPrefab,
                 mBulletSpawnPos.position,
                 mBulletSpawnPos.rotation);

            bullet.GetComponent<Bullet>().DamagePerHit = DamagePerHit;

            bullet.GetComponent<Rigidbody>().velocity = mBulletSpawnPos.forward * ShootForce;
        }

    }

}
