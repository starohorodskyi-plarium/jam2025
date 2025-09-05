using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject goodTargetPrefab;
    public GameObject badTargetPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    [Range(0f, 1f)]
    public float badTargetChance = 0.5f;
    public Transform[] spawnPoints;

    private float timer;

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnWave();
            timer = spawnInterval;
        }
    }

    private void SpawnWave()
    {
        foreach (Transform point in spawnPoints)
        {
            if (point.childCount > 0)
            {
                for (int i = point.childCount - 1; i >= 0; i--)
                {
                    Destroy(point.GetChild(i).gameObject);
                }
            }
            
            var isBad = Random.value < badTargetChance;
            var prefab = isBad ? badTargetPrefab : goodTargetPrefab;
            
            Instantiate(prefab, point.position, prefab.transform.rotation, point);
        }
    }
}
