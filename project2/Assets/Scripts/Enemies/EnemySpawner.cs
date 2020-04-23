using System.Collections;
﻿using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public int respawnTime = 0;
    public GameObject enemyPrefab;
    private GameObject spawnedEnemy;


    // Start is called before the first frame update
    void Start() {
        // Spawn initial enemy
        SpawnEnemy();
        StartCoroutine(SpawnEnemyAfterPeriod());
    }

    private void SpawnEnemy() {
        spawnedEnemy = Instantiate(enemyPrefab,
                                    transform.position,
                                    Quaternion.Euler(0, Random.Range(0, 360), 0),
                                    transform);
        spawnedEnemy.layer = LayerMask.NameToLayer("Enemies");
    }

    private IEnumerator SpawnEnemyAfterPeriod() {
        while (true) {
            yield return new WaitForSeconds(respawnTime);
            if (spawnedEnemy == null) {
                SpawnEnemy();
            }
        }
    }
}
