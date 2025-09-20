using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Obstacle
{
    public class ObstacleReaction : MonoBehaviour
    {
        [Header("Reaction Prefab")]
        [FormerlySerializedAs("reactionPrefab")] [SerializeField] private GameObject _reactionPrefab;

        public void Hit(float impactDelay, Vector3 collisionPoint)
        {
            Debug.Log($"{gameObject.name} was hit!");
       
            StartCoroutine(HitRoutine());
            return;

            IEnumerator HitRoutine()
            {
                yield return new WaitForSeconds(impactDelay);
                CreateReaction(collisionPoint);
            }
        }

        private void CreateReaction(Vector3 collisionPoint)
        {
            if (!_reactionPrefab)
                Debug.LogWarning($"{nameof(ObstacleReaction)}: reactionPrefab is not assigned.", this);
            else 
                Instantiate(_reactionPrefab, collisionPoint, Quaternion.identity);
        }
    }
}
