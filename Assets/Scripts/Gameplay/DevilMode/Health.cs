using System;
using Gun;
using Platform;
using UI.DevilificationProgress;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.DevilMode
{
    public class Health : MonoBehaviour
    {
        [FormerlySerializedAs("maxHealth")] [SerializeField, Min(1)] private int _maxHealth = 100;
        [FormerlySerializedAs("currentHealth")] [SerializeField] private int _currentHealth;
        [FormerlySerializedAs("isPlayer")] [SerializeField] private bool _isPlayer = true;

        public static event Action PlayerDied;
        public static Action<int> GetDamage;

        private bool _isDead;

        private void Awake()
        {
            _currentHealth = Mathf.Clamp(_currentHealth <= 0 ? _maxHealth : _currentHealth, 0, _maxHealth);
            _isDead = _currentHealth <= 0;
        }

        private void OnEnable() => 
            GetDamage += Remove;

        private void OnDisable() => 
            GetDamage -= Remove;

        private void OnValidate()
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
            UpdateProgressHealth();
        }

        private void Remove(int amount)
        {
            if (amount <= 0 || _isDead) 
                return;
            
            _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
            CameraShake.TriggerShake?.Invoke();
            UpdateProgressHealth();
            
            WGVibration.Vibrate(50);
            
            if (_currentHealth <= 0) 
                Die();
        }

        private void UpdateProgressHealth() =>
            DevilificationProgress.OnSetSmooth?.Invoke(_currentHealth/(float)_maxHealth);

        private void Die()
        {
            if (_isDead) 
                return;

            _isDead = true;
            
            if (_isPlayer) 
                PlayerDied?.Invoke();
        }
    }
}
