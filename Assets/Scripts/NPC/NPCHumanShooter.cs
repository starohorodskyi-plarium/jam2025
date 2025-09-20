using System.Collections;
using DG.Tweening;
using Gameplay.DevilMode;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class NPCHumanShooter : MonoBehaviour
    {
        [Header("Delays (seconds)")]
        [FormerlySerializedAs("initialDelayMin")] [SerializeField, Min(0f)] private float _initialDelayMin = 0f;
        [FormerlySerializedAs("initialDelayMax")] [SerializeField, Min(0f)] private float _initialDelayMax = 2f;
        [FormerlySerializedAs("betweenShotsDelayMin")] [SerializeField, Min(0f)] private float _betweenShotsDelayMin = 0.5f;
        [FormerlySerializedAs("betweenShotsDelayMax")] [SerializeField, Min(0f)] private float _betweenShotsDelayMax = 2f;
        
        [Header("Damage")]
        [FormerlySerializedAs("damageMin")] [SerializeField, Min(1)] private int _damageMin = 5;
        [FormerlySerializedAs("damageMax")] [SerializeField, Min(1)] private int _damageMax = 15;
        
        [Header("Projectile")]
        [FormerlySerializedAs("projectilePrefab")] [SerializeField] private GameObject _projectilePrefab;
        [FormerlySerializedAs("projectileSpawnPoint")] [SerializeField] private Transform _projectileSpawnPoint;
        [FormerlySerializedAs("projectileSpeed")] [SerializeField, Min(0.1f)] private float _projectileSpeed = 10f; // units per second
        [FormerlySerializedAs("projectileEase")] [SerializeField] private Ease _projectileEase = Ease.Linear;
        
        [Header("Shot VFX")]
        [FormerlySerializedAs("shotVfxPrefab")] [SerializeField] private GameObject _shotVfxPrefab;
        [FormerlySerializedAs("shotVfxLifetime")] [SerializeField, Min(0f)] private float _shotVfxLifetime = 2f;

        private Coroutine _shootingCoroutine;

        private void OnEnable()
        {
            DevilModeScenario.DevilModeActivated += HandleDevilModeActivated;
            DevilModeScenario.DevilModeDeactivated += HandleDevilModeDeactivated;
        }

        private void OnDisable()
        {
            DevilModeScenario.DevilModeActivated -= HandleDevilModeActivated;
            DevilModeScenario.DevilModeDeactivated -= HandleDevilModeDeactivated;
            StopShooting();
        }

        private void HandleDevilModeActivated() => 
            StartShooting();

        private void HandleDevilModeDeactivated() => 
            StopShooting();

        private void StartShooting()
        {
            StopShooting();
            _shootingCoroutine = StartCoroutine(ShootingLoop());
        }

        private void StopShooting()
        {
            if (_shootingCoroutine == null) 
                return;
            
            StopCoroutine(_shootingCoroutine);
            _shootingCoroutine = null;
        }

        private IEnumerator ShootingLoop()
        {
            var initialDelay = Random.Range(Mathf.Min(_initialDelayMin, _initialDelayMax), Mathf.Max(_initialDelayMin, _initialDelayMax));
            yield return new WaitForSeconds(initialDelay);

            while (true)
            {
                PerformShot();
                var delay = Random.Range(Mathf.Min(_betweenShotsDelayMin, _betweenShotsDelayMax), Mathf.Max(_betweenShotsDelayMin, _betweenShotsDelayMax));
                yield return new WaitForSeconds(delay);
            }
        }

        private void PerformShot()
        {
            SpawnShotVfx();
            SpawnAndLaunchProjectileTowardsCamera();
        }

        private void SpawnShotVfx()
        {
            if (!_shotVfxPrefab) 
                return;
            
            var parent = transform;
            var spawnPosition = _projectileSpawnPoint ? _projectileSpawnPoint.position : parent.position;
            var spawnRotation = _projectileSpawnPoint ? _projectileSpawnPoint.rotation : parent.rotation;

            var vfxInstance = Instantiate(_shotVfxPrefab, spawnPosition, spawnRotation, parent);
            
            if (_shotVfxLifetime > 0f) 
                Destroy(vfxInstance, _shotVfxLifetime);
        }

        private void SpawnAndLaunchProjectileTowardsCamera()
        {
            if (!_projectilePrefab) 
                return;
            
            var mainCamera = Camera.main;
            if (!mainCamera)
                return;

            var start = _projectileSpawnPoint ? _projectileSpawnPoint.position : transform.position;
            var target = mainCamera.transform.position;

            var projectileInstance = Instantiate(_projectilePrefab, start, Quaternion.identity);

            // Rotate projectile to face the camera at spawn
            var forward = (target - start).normalized;
            if (forward.sqrMagnitude > 1e-6f) 
                projectileInstance.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

            var distance = Vector3.Distance(start, target);
            if (distance <= 0.001f)
            {
                OnProjectileArrived(projectileInstance);
                return;
            }

            var speed = Mathf.Max(0.0001f, _projectileSpeed);
            var duration = distance / speed;

            projectileInstance.transform.DOMove(target, duration)
                .SetEase(_projectileEase)
                .OnComplete(() => OnProjectileArrived(projectileInstance));
        }

        private void OnProjectileArrived(GameObject projectileInstance)
        {
            var min = Mathf.Max(1, Mathf.Min(_damageMin, _damageMax));
            var max = Mathf.Max(min, Mathf.Max(_damageMin, _damageMax));
            var damageAmount = Random.Range(min, max + 1);

            Health.GetDamage?.Invoke(damageAmount);

            if (projectileInstance) 
                Destroy(projectileInstance);
        }
    }
}
