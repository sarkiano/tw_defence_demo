using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private float enemyRespawnInterval;
    [SerializeField] private float waveRespawnInterval;

    public static GameObject[] flags;
    public static Transform enemyStartPlace;
    public static List<GameObject> createdEnemies;

    public static float castleHealth;
    public static bool gameEnded;
    public static int totalEnemiesKilled;
    public static int waveEnemiesKilled;
    public static int waveIndex;
    public static float money; // - What inspired you to build a second Krusty Krab right next door to the original?
                               // - Money

    private static bool waveEnded;
    private static float difficultyMultiplier;

    private void Awake()
    {
        gameEnded = false;
        waveEnded = true;
        castleHealth = 1000f;
        difficultyMultiplier = 1f;
        waveIndex = 1;
        totalEnemiesKilled = 0;
        waveEnemiesKilled = 0;
        money = 0f;
        createdEnemies = new List<GameObject>();
        flags = GameObject.FindGameObjectsWithTag("flags");
        enemyStartPlace = GameObject.FindGameObjectWithTag("Respawn").transform;

        // Sort flags
        GameObject[] sorted_flags = new GameObject[flags.Length];
        Regex re = new Regex(@"\d+");
        for (int i = 0; i < flags.Length; i++)
        {
            int index = int.Parse(re.Match(flags[i].name).ToString());
            sorted_flags[index] = flags[i];
        }
        flags = (GameObject[]) sorted_flags.Clone();
    }

    private void Start()
    {
        StartCoroutine(SpawnWaves());
        InvokeRepeating("CheckGameOver", .0f, .2f);
    }

    private void CheckGameOver()
    {
        if (castleHealth <= 0f)
        {
            gameEnded = true;
            UIScript.Instance.RestartMenu();
            CancelInvoke("CheckGameOver");
        }
    }

    IEnumerator SpawnWaves()
    {
        while(!gameEnded)
        {
            if (waveEnded)
            {
                waveEnded = false;
                //Debug.Log($"Wave {waveIndex} incoming!");
                int enemyCount = Random.Range(waveIndex, waveIndex + 10);
                for (int i = 0; i < enemyCount; i++)
                {
                    var new_enemy = Instantiate(enemy, enemyStartPlace.position, enemyStartPlace.rotation);
                    new_enemy.tag = "Enemy";
                    new_enemy.Health = 10f * difficultyMultiplier;
                    new_enemy.Speed = 2f * difficultyMultiplier;
                    new_enemy.Damage = 0.2f * difficultyMultiplier;
                    new_enemy.Gold = Mathf.Round(10f * difficultyMultiplier);
                    new_enemy.TargetIndex = 0;
                    new_enemy.Id = i;
                    yield return new WaitForSeconds(Random.Range(0.3f, enemyRespawnInterval));
                }
            }
            else
            {
                if (waveEnemiesKilled == createdEnemies.Count)
                {
                    waveIndex++;
                    waveEnded = true;
                    waveEnemiesKilled = 0;
                    difficultyMultiplier += 0.1f;
                    createdEnemies = new List<GameObject>();
                }
                yield return new WaitForSeconds(waveRespawnInterval);
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }
}
