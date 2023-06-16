using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Vector3[] spawnPositions;

    private bool isTriggered;

    void Start()
    {
        isTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //print("enter");
        if (!isTriggered && other.CompareTag("PlayerEventTrigger"))
        {
            SpawnEnemy();
            isTriggered = true;
        }
    }

    private void SpawnEnemy()
    {
        foreach (Vector3 spawnPosition in spawnPositions)
        {
            print(spawnPosition.x);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.SetParent(this.transform, false);
            enemy.GetComponent<EnemyScript>().isActive = true;
            GameManagerScript.instance.enemyList.Add(enemy);
        }
    }

}
