using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Enemy[] enemies;
    int currentWave;
    int enemyCountForWave = 100;
    float difficultyFactor = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        currentWave = 1;
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCountForWave; i++)
        {
            var enemy = enemies[Random.Range(0, enemies.Length)];
            Vector3 spawnPos = new Vector3(Random.Range(-500f, 500f), 10f, Random.Range(-500f, 500f));
            Instantiate(enemy.enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

}

[System.Serializable]
public class Enemy
{
    public string name;
    public GameObject enemyPrefab;
}