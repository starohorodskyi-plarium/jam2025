using UnityEngine;

namespace MonoControllers
{
    public class ProjectileSpawnController : MonoBehaviour
    {
        [SerializeField] private ProjectileController _projectilePrefab;

        public void SpawnProjectile(ProjectileData projectileData)
        {
            var direction = projectileData.EndPosition - projectileData.StartPosition;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            
            var projectile = Instantiate(_projectilePrefab, projectileData.StartPosition, rotation);
            
            projectile.Initialize(projectileData);
        }
    }
}
