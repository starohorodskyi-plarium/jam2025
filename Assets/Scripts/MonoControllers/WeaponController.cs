using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace MonoControllers
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private ShootCallback _shootCallback;
        [SerializeField] private ProjectileController _projectileTemplate;
        [SerializeField] private Transform _fireOriginPoint;
        [SerializeField] private float _fireRateSec = .1f;
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private float _reloadDurationSec = 1f;
        [SerializeField] private int _maxAmmo = 10;
        [SerializeField] private LayerMask _hitMask;

        public UnityEvent OnShoot;
        public UnityEvent OnReloadStart;
        public UnityEvent OnReloadEnd;
        
        private int _currentAmmo;
        private DateTime? _reloadEndTime;
        private DateTime? _lastFireTime;
        private Vector3 _mousePosition;
        
        private void OnValidate()
        {
            Assert.IsNotNull(_projectileTemplate);
            Assert.IsNotNull(_shootCallback);
        }

        private void Start() => 
            _currentAmmo = _maxAmmo;

        private void OnEnable() => 
            _shootCallback.shootEvent.AddListener(Fire);

        private void OnDisable() => 
            _shootCallback.shootEvent.RemoveListener(Fire);

        private void Update()
        {
            var mouseRay = Camera.main?.ScreenPointToRay(Input.mousePosition);
            if (mouseRay == null)
                return;

            _mousePosition = Physics.Raycast(mouseRay.Value, out var mouseRayHit, _maxDistance, _hitMask)
                ? mouseRayHit.point
                : mouseRay.Value.origin + mouseRay.Value.direction * _maxDistance;
        }

        private void Fire()
        {
            if (_reloadEndTime != null && _reloadEndTime.Value > DateTime.Now)
                return;
            
            if (_lastFireTime != null && DateTime.Now - _lastFireTime.Value < TimeSpan.FromSeconds(_fireRateSec))
                return;
            
            if (_reloadEndTime != null)
            {
                _reloadEndTime = null;
                _currentAmmo = _maxAmmo;
            }

            var rayDirection = _mousePosition - _fireOriginPoint.position;
            
            var startPosition = _fireOriginPoint.position;
            var endPosition = Physics.Raycast(startPosition, rayDirection, out var hitInfo, _maxDistance, _hitMask)
                ? hitInfo.point
                : _fireOriginPoint.position + _fireOriginPoint.forward * _maxDistance;

            _lastFireTime = DateTime.Now;
            _currentAmmo--;
            
            if (_currentAmmo <= 0)
                _reloadEndTime = DateTime.Now.AddSeconds(_reloadDurationSec);
            

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_mousePosition, 1f);
        }
    }
}
