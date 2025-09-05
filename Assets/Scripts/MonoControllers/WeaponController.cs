using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace MonoControllers
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private ProjectileController _projectileTemplate;
        [SerializeField] private Transform _fireOriginPoint;
        [SerializeField] private float _fireRateSec = .1f;
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private LayerMask _hitMask;

        private DateTime? _lastFireTime;

        private void OnValidate() => 
            Assert.IsNotNull(_projectileTemplate);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                Fire();
        }

        private void Fire()
        {
            if (_lastFireTime != null && DateTime.Now - _lastFireTime.Value < TimeSpan.FromSeconds(_fireRateSec))
                return;

            var startPosition = _fireOriginPoint.position;
            var endPosition = Physics.Raycast(startPosition, _fireOriginPoint.forward, out var hitInfo, _maxDistance, _hitMask)
                ? hitInfo.point
                : _fireOriginPoint.position + _fireOriginPoint.forward * _maxDistance;

            _lastFireTime = DateTime.Now;

            var direction = endPosition - startPosition;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            
            var projectile = Instantiate(_projectileTemplate, _fireOriginPoint.transform.position, rotation);

            var projectileData = new ProjectileData
            {
                StartPosition = startPosition,
                EndPosition = endPosition,
            };
            
            projectile.Initialize(projectileData);
        }
    }
}
