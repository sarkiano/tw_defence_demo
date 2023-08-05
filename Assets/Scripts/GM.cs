using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {
    [SerializeField] private float enemyRespawnInterval;
    [SerializeField] private float waveRespawnInterval;

    public static GameObject[] flags;
    public static Transform enemyStartPlace;
    public static int createdEnemies;

    public static float castleHealth;
    public static bool gameEnded;
    public static int totalEnemiesKilled;
    public static int waveEnemiesKilled;
    public static int waveIndex;
    public static float money; // - What inspired you to build a second Krusty Krab right next door to the original?
                               // - Money

    private static bool _waveEnded;
    private static float _difficultyMultiplier;
    private static int[] _weights;
    private static string[] _enemies;

    private void Awake() {
        gameEnded = false;
        _waveEnded = true;
        castleHealth = 1000f;
        _difficultyMultiplier = 1f;
        waveIndex = 1;
        totalEnemiesKilled = 0;
        waveEnemiesKilled = 0;
        money = 0f;
        createdEnemies = 0;
        flags = GameObject.FindGameObjectsWithTag("flags");
        enemyStartPlace = GameObject.FindGameObjectWithTag("Respawn").transform;
        _weights = (int[])System.Enum.GetValues(typeof(EnemyTypesEnum));
        _enemies = System.Enum.GetNames(typeof(EnemyTypesEnum));

        // Sort flags
        GameObject[] sorted_flags = new GameObject[flags.Length];
        Regex re = new Regex(@"\d+");
        for (int i = 0; i < flags.Length; i++) {
            int index = int.Parse(re.Match(flags[i].name).ToString());
            sorted_flags[index] = flags[i];
        }
        flags = sorted_flags;
    }

    private void Start() {
        StartCoroutine(SpawnWaves());
        InvokeRepeating(nameof(CheckForGameOver), .0f, .2f);
    }

    private void CheckForGameOver() {
        if (castleHealth <= 0f) {
            castleHealth = 0f;
            gameEnded = true;
            CancelInvoke(nameof(CheckForGameOver));
            UIScript.Instance.RestartMenu();
        }
    }

    IEnumerator SpawnWaves() {
        while(!gameEnded) {
            if (_waveEnded) {
                _waveEnded = false;
                Debug.Log($"Wave {waveIndex} incoming!");
                int enemyCount = Random.Range(waveIndex, waveIndex + 10);
                for (int i = 0; i < enemyCount; i++) {
                    SpanwEnemies(i);
                    yield return new WaitForSeconds(Random.Range(0.3f, enemyRespawnInterval));
                }
            }
            else {
                if (waveEnemiesKilled == createdEnemies) {
                    waveIndex++;
                    _waveEnded = true;
                    waveEnemiesKilled = 0;
                    _difficultyMultiplier += 0.1f;
                    createdEnemies = 0;
                }
                //if (waveEnemiesKilled != createdEnemies) {
                //    Debug.Log("Huston we have a problem!");
                //}
                yield return new WaitForSeconds(waveRespawnInterval);
            }
        }
    }

    private void SpanwEnemies(int id)
    {
        GameObject new_enemy = null;
        int totalWeight = 0;

        foreach (int enemyWeight in System.Enum.GetValues(typeof(EnemyTypesEnum))) {
            totalWeight += enemyWeight;
        }
        
        int randomNumber = Random.Range(0, totalWeight);
        foreach (var enemyType in _enemies.Zip(_weights, (name, weight) => new { name, weight })) {
            if (randomNumber < enemyType.weight) {
                new_enemy = PoolObjects.Instance.GetPooledObject((EnemyTypesEnum)System.Enum.Parse(typeof(EnemyTypesEnum), enemyType.name));
                Debug.Log($"enemy - {new_enemy}");
                break;
            }
            totalWeight -= enemyType.weight;
        }
        if (!new_enemy) {
            Debug.Log("No enemy from random, sending hardcoded enemy type");
            new_enemy = PoolObjects.Instance.GetPooledObject(EnemyTypesEnum.ORC);
        }

        //var new_enemy = Instantiate(enemy, enemyStartPlace.position, enemyStartPlace.rotation);
        new_enemy.transform.SetPositionAndRotation(enemyStartPlace.position, enemyStartPlace.rotation);
        new_enemy.tag = "Enemy";
        Enemy enemyScript = new_enemy.GetComponent<Enemy>();
        enemyScript.Health = 10f * _difficultyMultiplier;
        enemyScript.Speed = 2f * _difficultyMultiplier;
        enemyScript.Damage = 0.2f * _difficultyMultiplier;
        enemyScript.Gold = Mathf.Round(10f * _difficultyMultiplier);
        enemyScript.TargetIndex = 0;
        enemyScript.Id = id;
        enemyScript.CleanseEnemy();
        new_enemy.SetActive(true);
    }

    public void RestartGame() {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }
}
