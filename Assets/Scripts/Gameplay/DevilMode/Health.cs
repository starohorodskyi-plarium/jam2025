using System;
using Gun;
using UI.DevilificationProgress;
using UnityEngine;

namespace Gameplay.DevilMode
{
    public class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private bool isPlayer = true;

        public static event Action PlayerDied;
    
        public static Action<int> GetDamage;

        private bool _isDead;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public bool IsDead => _isDead;

        private void Awake()
        {
            currentHealth = Mathf.Clamp(currentHealth <= 0 ? maxHealth : currentHealth, 0, maxHealth);
            _isDead = currentHealth <= 0;
        }

        private void OnEnable()
        {
            GetDamage += Remove;
        }

        private void OnDisable()
        {
            GetDamage -= Remove;
        }

        private void OnValidate()
        {
            if (maxHealth < 1) maxHealth = 1;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateProgressHealth();
        }

        public void Add(int amount)
        {
            if (amount <= 0 || _isDead) return;
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            UpdateProgressHealth();
        }

        public void Remove(int amount)
        {
            if (amount <= 0 || _isDead) return;
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
            CameraShake.TriggerShake?.Invoke();
        
            UpdateProgressHealth();
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Kill()
        {
            if (_isDead) return;
            currentHealth = 0;
            UpdateProgressHealth();
            Die();
        }

        private void UpdateProgressHealth() =>
            DevilificationProgress.OnSetSmooth?.Invoke(currentHealth/(float)maxHealth);

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;
            if (isPlayer)
            {
                PlayerDied?.Invoke();
            }
        }
    }
}
