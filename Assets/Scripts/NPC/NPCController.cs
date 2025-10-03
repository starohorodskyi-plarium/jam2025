using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace NPC
{
    public class NPCController : MonoBehaviour
    {
        private enum Faction
        {
            Ally,
            Enemy
        }

        public enum Gender
        {
            Male,
            Female
        }

        private Transform point1;
        private Transform point2;
        private Transform target;

        private const float MoveSpeed = 0.5f;
        private const float ReachThreshold = 0.1f; // how close is "arrived"

        private SpriteRenderer spriteRenderer;
        
        [Header("Character Settings")]
        [FormerlySerializedAs("faction")] [SerializeField] private Faction _faction = Faction.Enemy;
        [FormerlySerializedAs("gender")] [SerializeField] private Gender _gender = Gender.Male;
        [FormerlySerializedAs("allyHitEffect")] [SerializeField] private GameObject _allyHitEffect;
        [FormerlySerializedAs("enemyHitEffect")] [SerializeField] private GameObject _enemyHitEffect;
        [FormerlySerializedAs("soundPlayerPrefab")] [SerializeField] private GameObject _soundPlayerPrefab;
        [FormerlySerializedAs("spriteDirection")] [SerializeField] private NPCSpriteDirection _spriteDirection;

        private void Awake() => 
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        private void Update() => 
            Move();

        public void Hit(float impactDelay)
        {
            Debug.Log($"{gameObject.name} was hit!");

            StartCoroutine(HitRoutine());
            return;

            IEnumerator HitRoutine()
            {
                yield return new WaitForSeconds(impactDelay);
                PlayDeathEffects();
                Destroy(gameObject);
            }
        }
        
        public void Hit(float impactDelay, Action onHit)
        {
            Debug.Log($"{gameObject.name} was hit!");

            StartCoroutine(HitRoutine());
            return;

            IEnumerator HitRoutine()
            {
                yield return new WaitForSeconds(impactDelay);
                PlayDeathEffects();
                onHit?.Invoke();
                Destroy(gameObject);
            }
        }

        public void SetWaypoints(Transform p1, Transform p2)
        {
            point1 = p1;
            point2 = p2;

            // pick 0 or 1
            var random = Random.Range(0, 2);

            if (random == 0)
            {
                transform.position = point1.position;
                target = point2;
            }
            else
            {
                transform.position = point2.position;
                target = point1;
            }

            if (!spriteRenderer)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Move()
        {
            if (!point1 || !point2 || !target)
                return;

            // Move
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                MoveSpeed * Time.deltaTime
            );

            // Flip sprite based on movement direction (x axis)
            var dir = target.position - transform.position;
            if (spriteRenderer)
            {
                switch (dir.x)
                {
                    case > 0.01f:
                        _spriteDirection.SetDirection(NPCSpriteDirection.Direction.Right);
                        break;
                    case < -0.01f:
                        _spriteDirection.SetDirection(NPCSpriteDirection.Direction.Left);
                        break;
                }
            }

            // Switch target if reached
            if (Vector3.Distance(transform.position, target.position) < ReachThreshold)
                target = target == point1 ? point2 : point1;
        }

        private void PlayDeathEffects()
        {
            var effectPrefab = _faction == Faction.Ally ? _allyHitEffect : _enemyHitEffect;
            if (effectPrefab)
                Instantiate(effectPrefab, transform.position, Quaternion.identity);

            if (!_soundPlayerPrefab)
                return;

            var soundObj = Instantiate(_soundPlayerPrefab, transform.position, Quaternion.identity);
            var reaction = soundObj.GetComponent<NPCShotAudioReaction>();

            if (!reaction)
                return;

            if (_faction == Faction.Ally)
                reaction.PlayHumanDeath(_gender);
            else
                reaction.PlayDemonDeath(_gender);
        }
    }
}
