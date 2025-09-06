using System;
using Gun;
using UnityEngine;
using UnityEngine.Events;

namespace MonoControllers
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Transform _fireOriginPoint;
        [SerializeField] private float _fireRateSec = .1f;
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private float _reloadDurationSec = 1f;
        [SerializeField] private int _maxAmmo = 10;
        [SerializeField] private LayerMask _hitMask;

        public UnityEvent<ProjectileData> OnShoot;
        public UnityEvent<WeaponAmmoData> OnAmmoChanged;
        public UnityEvent OnReloadStart;
        public UnityEvent OnReloadEnd;
        
        [SerializeField]
        private int _currentAmmo;
        private DateTime? _reloadEndTime;
        private DateTime? _lastFireTime;
        private Vector3 _mousePosition;

        private bool Reloading => _reloadEndTime != null && _reloadEndTime.Value > DateTime.Now;
        private bool FireDelay => _lastFireTime != null && DateTime.Now - _lastFireTime.Value < TimeSpan.FromSeconds(_fireRateSec);

        private void Start()
        {
            _currentAmmo = _maxAmmo;
            DispatchAmmoChanged();
        }

        private void Update()
        {
            UpdateAfterReload();
            
            if (GameManager.Instance.InputEnabled)
                UpdateMousePosition();
        }

        public void TryFire()
        {
            if (Reloading || FireDelay)
                return;
    
            UpdateAfterShoot();

            CalculateTrajectory(out var projectileData, out var hitObject);

            OnShoot?.Invoke(projectileData);

            if (hitObject != null)
            {
                HandleTargetHit(hitObject);
            }

            DispatchAmmoChanged();
        }
        
        private void HandleTargetHit(GameObject hitObject)
        {
            if (hitObject.CompareTag("Friend"))
            {
                GameManager.Instance.SubtractTime();
                
                hitObject.GetComponent<NPCController>()?.Hit();
            }
            else if (hitObject.CompareTag("Enemy"))
            {
                GameManager.Instance.AddTime();
                
                hitObject.GetComponent<NPCController>()?.Hit();
            }
        }

        private void UpdateAfterReload()
        {
            if (Reloading)
                return;
            
            if (_reloadEndTime == null) 
                return;
            
            _reloadEndTime = null;
            _currentAmmo = _maxAmmo;
            
            OnReloadEnd?.Invoke();
            DispatchAmmoChanged();
        }

        private void UpdateAfterShoot()
        {
            _lastFireTime = DateTime.Now;
            _currentAmmo--;

            if (_currentAmmo > 0) 
                return;
            
            _reloadEndTime = DateTime.Now.AddSeconds(_reloadDurationSec);
            
            OnReloadStart?.Invoke();
        }

        private void UpdateMousePosition()
        {
            var mouseRay = Camera.main?.ScreenPointToRay(Input.mousePosition);
            if (mouseRay == null)
                return;

            _mousePosition = Physics.Raycast(mouseRay.Value, out var mouseRayHit, _maxDistance, _hitMask)
                ? mouseRayHit.point
                : mouseRay.Value.origin + mouseRay.Value.direction * _maxDistance;
        }

        private void CalculateTrajectory(out ProjectileData projectileData, out GameObject hitObject)
        {
            var rayDirection = _mousePosition - _fireOriginPoint.position;
            var startPosition = _fireOriginPoint.position;

            if (Physics.Raycast(startPosition, rayDirection, out var hitInfo, _maxDistance, _hitMask))
            {
                projectileData = new ProjectileData
                {
                    StartPosition = startPosition,
                    EndPosition = hitInfo.point,
                };
                hitObject = hitInfo.collider.gameObject; // THIS is the object the ray hit
            }
            else
            {
                projectileData = new ProjectileData
                {
                    StartPosition = startPosition,
                    EndPosition = _fireOriginPoint.position + _fireOriginPoint.forward * _maxDistance,
                };
                hitObject = null;
            }
        }

        private void DispatchAmmoChanged()
        {
            OnAmmoChanged?.Invoke(new WeaponAmmoData
            {
                Current = _currentAmmo,
                Max = _maxAmmo,
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_mousePosition, 1f);
        }
    }
}
