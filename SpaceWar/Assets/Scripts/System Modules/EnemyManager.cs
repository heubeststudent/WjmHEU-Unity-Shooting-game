using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    public GameObject RandomEnemy =>enemyList.Count == 0 ? null: enemyList[Random.Range(0,enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;
    [SerializeField] bool spawnEnemy = true;
    //[SerializeField] GameObject waveUI;
    [SerializeField] GameObject[] enemyPerfabs;
    [SerializeField] float timeBetweenSpawns = 1f;
    [SerializeField] int minEnemyAmount = 4;
    [SerializeField] int maxEnemyAmount = 10;
    [SerializeField] float timeBetweenWaves = 1f;
    int waveNumber = 1;
    int enemyAmount = 1;
    WaitForSeconds waitTimeBetweenSpawns;
    WaitForSeconds waitTimeBetweenWaves;
    List<GameObject> enemyList;
    WaitUntil waitUntilNoEnemy;
    [Header("---- Boss Settings ----")]
    [SerializeField] GameObject bossPrefab;
    [SerializeField] int bossWaveNumber;

    protected override void Awake()
    {
        base.Awake();
        enemyList = new List<GameObject>();
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenWaves);
        waitUntilNoEnemy = new WaitUntil(() => enemyList.Count == 0);
    }

    IEnumerator Start()
    {
        while (spawnEnemy && GameManager.GameState != GameState.GameOver)
        {

            //waveUI.SetActive(true);

            yield return waitTimeBetweenWaves;

            //waveUI.SetActive(false);

            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
        
    }

    IEnumerator RandomlySpawnCoroutine()
    {
        if (waveNumber % bossWaveNumber == 0)
        {
            var boss = PoolManager.Release(bossPrefab);
            enemyList.Add(boss);
        }
        else
        {
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount+waveNumber/3, maxEnemyAmount);
            for (int i = 0; i <enemyAmount ;i++)
            {
                //var enemy = enemyPerfabs[Random.Range(0, enemyPerfabs.Length)]
                enemyList.Add( PoolManager.Instantiate(enemyPerfabs[Random.Range(0,enemyPerfabs.Length)]));
                yield return waitTimeBetweenSpawns;
            }
                
        }
        yield return waitUntilNoEnemy;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);


}
