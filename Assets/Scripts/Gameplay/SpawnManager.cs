using UnityEngine;
using Random = UnityEngine.Random;

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

    public void SpawnWave()
    {
        foreach (var point in spawnPoints)
        {
            if (point.childCount > 0)
            {
                for (var i = point.childCount - 1; i >= 0; i--)
                {
                    Destroy(point.GetChild(i).gameObject);
                }
            }
            
            var isBad = Random.value < badTargetChance;
            var prefab = isBad ? badTargetPrefab : goodTargetPrefab;
            
            Instantiate(prefab, point.position, prefab.transform.rotation, point);
        }
    }

    public void DestroyAll()
    {
        foreach (var point in spawnPoints)
        {
            if (point.childCount <= 0) 
                continue;
            
            for (var i = point.childCount - 1; i >= 0; i--)
            {
                Destroy(point.GetChild(i).gameObject);
            }
        }
    }
}
