using System.Collections.Generic;
using NPC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Prefab Collections")]
        public GameObject[] goodNpcPrefabs;
        public GameObject[] badNpcPrefabs;
    
        public SnailController[] snails;

        [Header("Spawn Settings")]
        public Transform[] spawnPoints;

        [Tooltip("How many baddies should spawn on this level.")]
        public int baddiesPerLevel = 2;

        public int EnemiesDefeatCount
        {
            get
            {
                var count = baddiesPerLevel;
                foreach (var point in spawnPoints)
                {
                    for (var i = 0; i < point.childCount; i++)
                    {
                        var child = point.GetChild(i).gameObject;
                        if (child.CompareTag("Enemy"))
                            count--;
                    }
                }

                return count;
            }
        }
    
        public void SpawnWave()
        {
            foreach (var snailController in snails)
                snailController.Activate();
        
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("No spawn points assigned!");
                return;
            }

            // Clean spawn points
            foreach (var point in spawnPoints)
            {
                for (int i = point.childCount - 1; i >= 0; i--)
                {
                    var child = point.GetChild(i);
                    if (child.name != "p1" && child.name != "p2")
                        Destroy(child.gameObject);
                }
            }

            // Clamp baddies count
            int numBaddies = Mathf.Clamp(baddiesPerLevel, 0, spawnPoints.Length);
            int numGoodies = spawnPoints.Length - numBaddies;

            // Make randomized spawn order
            List<bool> spawnOrder = new List<bool>();
            for (int i = 0; i < numBaddies; i++) spawnOrder.Add(true);
            for (int i = 0; i < numGoodies; i++) spawnOrder.Add(false);
            Shuffle(spawnOrder);

            // Spawn NPCs
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                var point = spawnPoints[i];
                var p1 = point.Find("p1");
                var p2 = point.Find("p2");

                if (p1 == null || p2 == null)
                {
                    Debug.LogWarning($"Spawn point {point.name} is missing p1 or p2!");
                    continue;
                }

                // Pick random prefab from correct pool
                GameObject prefab = spawnOrder[i]
                    ? badNpcPrefabs[Random.Range(0, badNpcPrefabs.Length)]
                    : goodNpcPrefabs[Random.Range(0, goodNpcPrefabs.Length)];

                // Spawn at p1
                var instance = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, point);

                // Assign waypoints
                var controller = instance.GetComponent<NPCController>();
                if (controller != null)
                    controller.SetWaypoints(p1, p2);
            }
        }

        public void DestroyAll()
        {
            foreach (var snailController in snails)
                snailController.Deactivate();
        
            foreach (var point in spawnPoints)
            {
                for (int i = point.childCount - 1; i >= 0; i--)
                {
                    var child = point.GetChild(i);
                    if (child.name != "p1" && child.name != "p2")
                        Destroy(child.gameObject);
                }
            }
        }

        public bool AllEnemiesDefeated()
        {
            foreach (var point in spawnPoints)
            {
                for (int i = 0; i < point.childCount; i++)
                {
                    var child = point.GetChild(i).gameObject;
                    if (child.CompareTag("Enemy"))
                        return false;
                }
            }
            return true;
        }

        // Fisher–Yates shuffle
        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }
    }
}