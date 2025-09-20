using System.Collections;
using UnityEngine;

namespace Gameplay.Obstacle
{
    public class ObstacleReaction : MonoBehaviour
    {
        [Header("Reaction Prefab")]
        [SerializeField] private GameObject reactionPrefab;

        public void Hit(float impactDelay, Vector3 collisionPoint)
        {
            Debug.Log($"{gameObject.name} was hit!");
       
            StartCoroutine(HitRoutine());

            IEnumerator HitRoutine()
            {
                yield return new WaitForSeconds(impactDelay);
                CreateReaction(collisionPoint);
            }
        }
    
        public void CreateReaction(Vector3 collisionPoint)
        {
            if (!reactionPrefab)
                Debug.LogWarning($"{nameof(ObstacleReaction)}: reactionPrefab is not assigned.", this);
            else 
                Instantiate(reactionPrefab, collisionPoint, Quaternion.identity);
        }
    }
}
