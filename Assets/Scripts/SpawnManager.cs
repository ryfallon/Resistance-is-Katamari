using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject player;
    public Enemy nearestEnemy;
    private Enemy[] enemyList;
    public HashSet<Enemy> inRangeSet;
    private float spawnRange = 200f;
    private float groupSpawnRange = 30f;
    private float yRange = 10f;
    public int enemyCount;
    public int assimilatedCount;
    public int destroyedCount;
    public int waveNumber = 2;
    public bool isSpawning = false;
    public float baseFiringRange = 30f;
    public float firingRange;
    public float spawnFactor = 1.5f;
    public float playerRadius = 3.5f;
    public float displayScale = 120f;

    public TextMeshProUGUI displayText;
    public TextMeshProUGUI countDisplayText;
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        PopulateLists();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        PopulateLists();
        countDisplayText.text = "Assimilated: " + assimilatedCount + " Ships\n" +
            "Destroyed: " + destroyedCount + " Ships\n" + 
            "Volume-based Katamari Radius: " + (playerRadius*displayScale).ToString("F1") + "m";
        
        //foreach (Enemy inRange in inRangeSet)
        //{
        //    if (inRange != null)
        //        displayText.text += (inRange.name + ": " + inRange.distanceToPlayer.ToString("F0") + "m\n");
        //}

        if (enemyCount == 0 && !isSpawning)
        {
            waveNumber = Mathf.RoundToInt(waveNumber * spawnFactor);
            SpawnEnemyWave(waveNumber);
        }
    }

    void PopulateLists()
    {
        assimilatedCount = 0;
        enemyList = FindObjectsOfType<Enemy>();
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        inRangeSet = new HashSet<Enemy>();
        firingRange = baseFiringRange + playerRadius;

        displayText.text = "Enemies remaining: " + enemyCount + "\n";
        float nearestDistance = 9999;
        foreach (Enemy enemy in enemyList)
        {
            if (!enemy.isAssimilated && !enemy.isInDestroyAnimation)
            {
                if (enemy.distanceToPlayer < firingRange)
                    inRangeSet.Add(enemy);
                displayText.text += enemy.shiptype + ": " + (enemy.distanceToPlayer*displayScale/1000).ToString("F1") + "km\n";
                if (enemy.distanceToPlayer < nearestDistance)
                {
                    nearestDistance = enemy.distanceToPlayer;
                    nearestEnemy = enemy;
                }
            } else if (enemy.isAssimilated)
            {
                assimilatedCount++;
            }
        }
    }

    private Vector3 GenerateSpawnPosition(Vector3 center, float range)
    {
        float spawnPosX = Random.Range(-range, range);
        float spawnPosZ = Random.Range(-range, range);
        float spawnPosY = Random.Range(-yRange, yRange);
        Vector3 randomPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ) + center;
        return randomPos;
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        Vector3 groupPos = Vector3.zero;
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (i % 6 == 0)
            {
                float groupDist = 0f;
                Vector3 newGroupPos;
                do
                {
                    newGroupPos = GenerateSpawnPosition(Vector3.zero, spawnRange);
                    groupDist = (newGroupPos - groupPos).magnitude;
                } while (groupDist < groupSpawnRange + 20);
                groupPos = newGroupPos;
            }
            int spawnType = i % 2 * 2;
            if (i % 12 == 0)
                spawnType = 1;
            if (i % 20 == 0)
                spawnType = 3;
            StartCoroutine(SpawnEnemy(spawnType, Random.Range(0f, 4f), groupPos));
        }
    }

    IEnumerator SpawnEnemy(int type, float delay, Vector3 groupPosition)
    {
        isSpawning = true;
        yield return new WaitForSeconds(delay);
        Vector3 position = GenerateSpawnPosition(groupPosition, groupSpawnRange);
        Quaternion rotation = Quaternion.LookRotation(player.transform.position - position, Vector3.up);
        Instantiate(enemyPrefabs[type], position, rotation);
        isSpawning = false;
    }
}
