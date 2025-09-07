using System.Collections;
using DG.Tweening;
using Gameplay.DevilMode;
using UnityEngine;

public class NPCHumanShooter : MonoBehaviour
{
    [Header("Delays (seconds)")]
    [SerializeField, Min(0f)] private float initialDelayMin = 0f;
    [SerializeField, Min(0f)] private float initialDelayMax = 2f;
    [SerializeField, Min(0f)] private float betweenShotsDelayMin = 0.5f;
    [SerializeField, Min(0f)] private float betweenShotsDelayMax = 2f;

    [Header("Damage")]
    [SerializeField, Min(1)] private int damageMin = 5;
    [SerializeField, Min(1)] private int damageMax = 15;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 10f; // units per second
    [SerializeField] private Ease projectileEase = Ease.Linear;

    [Header("Shot VFX")]
    [SerializeField] private GameObject shotVfxPrefab;
    [SerializeField, Min(0f)] private float shotVfxLifetime = 2f;

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

    private void HandleDevilModeActivated()
    {
        StartShooting();
    }

    private void HandleDevilModeDeactivated()
    {
        StopShooting();
    }

    private void StartShooting()
    {
        StopShooting();
        _shootingCoroutine = StartCoroutine(ShootingLoop());
    }

    private void StopShooting()
    {
        if (_shootingCoroutine != null)
        {
            StopCoroutine(_shootingCoroutine);
            _shootingCoroutine = null;
        }
    }

    private IEnumerator ShootingLoop()
    {
        float initialDelay = Random.Range(Mathf.Min(initialDelayMin, initialDelayMax), Mathf.Max(initialDelayMin, initialDelayMax));
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            PerformShot();
            float delay = Random.Range(Mathf.Min(betweenShotsDelayMin, betweenShotsDelayMax), Mathf.Max(betweenShotsDelayMin, betweenShotsDelayMax));
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
        if (!shotVfxPrefab) return;
        Transform parent = transform;
        Vector3 spawnPosition = projectileSpawnPoint ? projectileSpawnPoint.position : parent.position;
        Quaternion spawnRotation = projectileSpawnPoint ? projectileSpawnPoint.rotation : parent.rotation;

        GameObject vfxInstance = Instantiate(shotVfxPrefab, spawnPosition, spawnRotation, parent);
        if (shotVfxLifetime > 0f)
        {
            Destroy(vfxInstance, shotVfxLifetime);
        }
    }

    private void SpawnAndLaunchProjectileTowardsCamera()
    {
        if (!projectilePrefab) return;
        Camera mainCamera = Camera.main;
        if (!mainCamera) return;

        Vector3 start = projectileSpawnPoint ? projectileSpawnPoint.position : transform.position;
        Vector3 target = mainCamera.transform.position;

        GameObject projectileInstance = Instantiate(projectilePrefab, start, Quaternion.identity);

        // Rotate projectile to face the camera at spawn
        Vector3 forward = (target - start).normalized;
        if (forward.sqrMagnitude > 1e-6f)
        {
            projectileInstance.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        float distance = Vector3.Distance(start, target);
        if (distance <= 0.001f)
        {
            OnProjectileArrived(projectileInstance);
            return;
        }

        float speed = Mathf.Max(0.0001f, projectileSpeed);
        float duration = distance / speed;

        projectileInstance.transform.DOMove(target, duration)
            .SetEase(projectileEase)
            .OnComplete(() => OnProjectileArrived(projectileInstance));
    }

    private void OnProjectileArrived(GameObject projectileInstance)
    {
        int min = Mathf.Max(1, Mathf.Min(damageMin, damageMax));
        int max = Mathf.Max(min, Mathf.Max(damageMin, damageMax));
        int damageAmount = Random.Range(min, max + 1);

        Health.GetDamage?.Invoke(damageAmount);

        if (projectileInstance)
        {
            Destroy(projectileInstance);
        }
    }
}
