using System.Collections.Generic;
using NPC;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Prefab Collections")]
        [FormerlySerializedAs("goodNpcPrefabs")] public GameObject[] GoodNpcPrefabs;
        [FormerlySerializedAs("badNpcPrefabs")] public GameObject[] BadNpcPrefabs;
    
        [FormerlySerializedAs("snails")] public SnailController[] Snails;

        [Header("Spawn Settings")]
        [FormerlySerializedAs("spawnPoints")] public Transform[] SpawnPoints;

        [Tooltip("How many baddies should spawn on this level.")]
        [FormerlySerializedAs("baddiesPerLevel")] public int BaddiesPerLevel = 2;
        
        public List<NPCController> Npcs = new();

        public int EnemiesDefeatCount
        {
            get
            {
                var count = BaddiesPerLevel;
                foreach (var point in SpawnPoints)
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
            foreach (var snailController in Snails)
                snailController.Activate();
        
            if (SpawnPoints == null || SpawnPoints.Length == 0)
            {
                Debug.LogWarning("No spawn points assigned!");
                return;
            }

            // Clean spawn points
            foreach (var point in SpawnPoints)
            {
                for (var i = point.childCount - 1; i >= 0; i--)
                {
                    var child = point.GetChild(i);
                    if (child.name != "p1" && child.name != "p2")
                        Destroy(child.gameObject);
                }
            }

            // Clamp baddies count
            var numBaddies = Mathf.Clamp(BaddiesPerLevel, 0, SpawnPoints.Length);
            var numGoodies = SpawnPoints.Length - numBaddies;

            // Make randomized spawn order
            var spawnOrder = new List<bool>();
            for (var i = 0; i < numBaddies; i++) 
                spawnOrder.Add(true);
            
            for (var i = 0; i < numGoodies; i++) 
                spawnOrder.Add(false);
            
            Shuffle(spawnOrder);

            // Spawn NPCs
            for (var i = 0; i < SpawnPoints.Length; i++)
            {
                var point = SpawnPoints[i];
                var p1 = point.Find("p1");
                var p2 = point.Find("p2");

                if (p1 == null || p2 == null)
                {
                    Debug.LogWarning($"Spawn point {point.name} is missing p1 or p2!");
                    continue;
                }

                // Pick random prefab from correct pool
                var prefab = spawnOrder[i]
                    ? BadNpcPrefabs[Random.Range(0, BadNpcPrefabs.Length)]
                    : GoodNpcPrefabs[Random.Range(0, GoodNpcPrefabs.Length)];

                // Spawn at p1
                var instance = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, point);

                
                
                // Assign waypoints
                var controller = instance.GetComponent<NPCController>();
                if (controller != null)
                {
                    controller.SetWaypoints(p1, p2);
                    Npcs.Add(controller);
                }
            }
        }

        public void DestroyAll()
        {
            foreach (var snailController in Snails)
                snailController.Deactivate();
        
            foreach (var point in SpawnPoints)
            {
                for (var i = point.childCount - 1; i >= 0; i--)
                {
                    var child = point.GetChild(i);
                    if (child.name != "p1" && child.name != "p2")
                        Destroy(child.gameObject);
                }
            }
            
            Npcs.Clear();
        }

        public bool AllEnemiesDefeated()
        {
            foreach (var point in SpawnPoints)
            {
                for (var i = 0; i < point.childCount; i++)
                {
                    var child = point.GetChild(i).gameObject;
                    if (child.CompareTag("Enemy"))
                        return false;
                }
            }
            
            return true;
        }

        // Fisher–Yates shuffle
        private static void Shuffle<T>(List<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }
    }
}