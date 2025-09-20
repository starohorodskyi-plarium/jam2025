using System.Collections;
using Gameplay.Obstacle;
using UI.SnailUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class SnailController : MonoBehaviour
    {
        [Header("Reaction Prefab")]
        [FormerlySerializedAs("reactionPrefab")] [SerializeField] private GameObject _reactionPrefab;
        [FormerlySerializedAs("letter")] [SerializeField] private SnailLetter _letter;
        [FormerlySerializedAs("snailSprite")] [SerializeField] private SpriteRenderer _snailSprite;


        public void Activate() => 
            _snailSprite.enabled = true;

        public void Deactivate()
        {
            if(_snailSprite)
                _snailSprite.enabled = false;
        }
       
    
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
            {
                Instantiate(_reactionPrefab, collisionPoint, Quaternion.identity); 
                gameObject.SetActive(false);
                SnailProgress.LetterOpen?.Invoke(_letter);
            }
           
        }
    }
}
