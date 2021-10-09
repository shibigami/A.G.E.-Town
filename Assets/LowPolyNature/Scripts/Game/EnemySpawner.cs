using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] Enemies;

    public GameObject SpawnEffectPrefab;

    void Start()
    {
        foreach(var enemy in Enemies)
        {
            enemy.SetActive(false);
        }
    }

    public void Spawn()
    {
        foreach (var enemy in Enemies)
        {
            if(SpawnEffectPrefab != null)
            {
                Instantiate(SpawnEffectPrefab, enemy.transform.position, SpawnEffectPrefab.transform.rotation);
            }
        }

        StartCoroutine(ShowEnemies());


    }

    IEnumerator ShowEnemies()
    {
        yield return new WaitForSeconds(1.0f);

        foreach (var enemy in Enemies)
        {
            enemy.SetActive(true);
        }

    }
}
