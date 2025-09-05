using System;
using UnityEngine;

namespace MonoControllers
{
    public struct ProjectileData
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
    }
    
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _lifetimeSec = 5f;
        [SerializeField] private float _distanceToDestroy = 1f;

        private DateTime _destroyTime;
        private ProjectileData _projectileData;
        
        public void Initialize(ProjectileData projectileData) => 
            _projectileData = projectileData;
        
        private void Awake() => 
            _destroyTime = DateTime.Now.AddSeconds(_lifetimeSec);
        
        private void LateUpdate()
        {
            transform.position += transform.forward * _speed * Time.deltaTime;

            var shouldDestroy = DateTime.Now >= _destroyTime ||
                                Vector3.Distance(transform.position, _projectileData.EndPosition) <= _distanceToDestroy;  
            
            if (shouldDestroy) 
                Destroy(gameObject);
        }
    }
}
