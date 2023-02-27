using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Enemy[] enemies;
    public GameObject playerObj;
    int currentWave;
    float enemyCountForWave = 13f;
    public int enemiesRemainingThisWave;
    float difficultyFactor = 1.5f;
    public TMP_Text roundCounter;

    // Start is called before the first frame update
    void Start()
    {
        currentWave = 1;
        SpawnEnemies();
    }

    IEnumerator StartNextWave(int timer)
    {

        // All enemies are dead, start another wave.
        print("All enemies killed, wave " + currentWave + "finished.");

        // Remove all dead enemies from last wave.
        GameObject[] deadEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in deadEnemies)
        {
            Destroy(enemy, 2f);
        }

        // Update the settings so that the next wave is harder.
        enemyCountForWave *= difficultyFactor;
        enemyCountForWave = Mathf.Floor(enemyCountForWave);
        currentWave++;


        // Wait until the timer hits zero...
        while (timer != 0)
        {
            timer -= 1;
            roundCounter.text = "Wave " + currentWave + " starting in: " + timer.ToString();
            yield return new WaitForSeconds(1f);
        }


        SpawnEnemies();
        yield return null;
    }   

    void SpawnEnemies()
    {
        enemiesRemainingThisWave = 0;
        int enemiesSpawnedThisWave = 0;
        while (enemyCountForWave != enemiesSpawnedThisWave)
        {
            var enemy = enemies[Random.Range(0, enemies.Length)];
            Vector3 spawnPos = new Vector3(Random.Range(-500f, 500f), 10f, Random.Range(-500f, 500f));

            if (Vector3.Distance(spawnPos, playerObj.transform.position) < 200f)
            {
                GameObject spawnedEnemy = Instantiate(enemy.enemyPrefab, spawnPos, Quaternion.identity);
                spawnedEnemy.GetComponent<EnemyController>().currentHealth *= difficultyFactor;
                spawnedEnemy.GetComponent<EnemyController>().enemySpeed *= difficultyFactor;
                spawnedEnemy.GetComponent<EnemyController>().gameManager = this; // Set the gameManager script reference in the new enemy spawned.
                enemiesSpawnedThisWave++;
                spawnedEnemy.name = "Enemy " + enemiesSpawnedThisWave;
            }

        }
        enemiesRemainingThisWave = enemiesSpawnedThisWave;
    }

    public void KillEnemy()
    {
        print(gameObject.name + " killed. Total remaining for wave " + currentWave + " = " + enemiesRemainingThisWave);
        enemiesRemainingThisWave--;

        if (enemiesRemainingThisWave == 0)
        {
            StartCoroutine(StartNextWave(15));
        }
    }


}

[System.Serializable]
public class Enemy
{
    public string name;
    public GameObject enemyPrefab;
}