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
        private Vector3 _mousePosition;

        private bool IgnoreInputs => GameManager.Instance.CurrentState != GameManager.GameState.Playing;
        
        private void OnValidate() => 
            Assert.IsNotNull(_projectileTemplate);

        private void Update()
        {
            var mouseRay = Camera.main?.ScreenPointToRay(Input.mousePosition);
            if (mouseRay == null)
                return;

            var mousePosition = Physics.Raycast(mouseRay.Value, out var mouseRayHit, _maxDistance, _hitMask)
                ? mouseRayHit.point
                : mouseRay.Value.origin + mouseRay.Value.direction * _maxDistance;
            
            _mousePosition = mouseRayHit.point;
            
            if (!IgnoreInputs && Input.GetKeyDown(KeyCode.Mouse0))
                Fire(mousePosition);
        }

        private void Fire(Vector3 mousePosition)
        {
            if (_lastFireTime != null && DateTime.Now - _lastFireTime.Value < TimeSpan.FromSeconds(_fireRateSec))
                return;

            var rayDirection = mousePosition - _fireOriginPoint.position;
            
            var startPosition = _fireOriginPoint.position;
            var endPosition = Physics.Raycast(startPosition, rayDirection, out var hitInfo, _maxDistance, _hitMask)
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_mousePosition, 1f);
        }
    }
}
