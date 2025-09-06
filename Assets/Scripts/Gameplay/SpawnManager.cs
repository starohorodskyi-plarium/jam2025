using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject goodTargetPrefab;
    public GameObject badTargetPrefab;

    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float badTargetChance = 0.5f;
    public Transform[] spawnPoints;

    public void SpawnWave()
    {
        foreach (var point in spawnPoints)
        {
            // Clean up existing children (not p1/p2)
            for (var i = point.childCount - 1; i >= 0; i--)
            {
                var child = point.GetChild(i);
                if (child.name != "p1" && child.name != "p2")
                {
                    Destroy(child.gameObject);
                }
            }

            // Decide prefab
            var isBad = Random.value < badTargetChance;
            var prefab = isBad ? badTargetPrefab : goodTargetPrefab;
            
            // Get p1 and p2 transforms
            var p1 = point.Find("p1");
            var p2 = point.Find("p2");
            
            if (p1 == null || p2 == null)
            {
                Debug.LogWarning($"Spawn point {point.name} is missing p1 or p2!");
                continue;
            }

            // Spawn prefab
            var instance = Instantiate(prefab, p1.position, prefab.transform.rotation, point);
            
            if (p1 != null && p2 != null)
            {
                // Pass them into NpcController
                var controller = instance.GetComponent<NPCController>();
                if (controller != null)
                {
                    controller.SetWaypoints(p1, p2);
                }
            }
        }
    }

    public void DestroyAll()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        foreach (var point in spawnPoints)
        {
            for (int i = point.childCount - 1; i >= 0; i--)
            {
                var child = point.GetChild(i);
                if (child.name != "p1" && child.name != "p2")
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public bool AllEnemiesDefeated()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return true;

        foreach (var point in spawnPoints)
        {
            // Ignore p1/p2
            for (int i = 0; i < point.childCount; i++)
            {
                var child = point.GetChild(i).gameObject;
                if (child.CompareTag("Enemy"))
                    return false;
            }
        }

        return true;
    }
}