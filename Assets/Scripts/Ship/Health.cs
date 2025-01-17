using UnityEngine;
using System;

namespace ShipMotorika
{
    /// <summary>
    /// Здоровье корабля.
    /// </summary>
    public class Health : MonoBehaviour, ILoader, ISaver
    {
        /// <summary>
        /// Максимально допустимое значение здоровья корабля.
        /// </summary>
        [SerializeField] private int _maxHealth;
        public int MaxHealth => _maxHealth;

        /// <summary>
        /// Текущее значение здоровья корабля.
        /// </summary>
        [SerializeField] private int _currentHealth;
        public int CurrentHealth => _currentHealth;

        [Header("Collisions")]

        /// <summary>
        /// Минимальный обязательный урон при столкновениях.
        /// </summary>
        [SerializeField] private int _damageConstant;
        public int DamageConstant => _damageConstant;

        /// <summary>
        /// Множитель урона при столкновениях.
        /// </summary>
        [SerializeField] private float _damageMultiplier;
        public float DamageMultiplier => _damageMultiplier;

        /// <summary>
        /// Неуязвимость к урону.
        /// </summary>
        [SerializeField] private bool _isIndestructible;

        public event Action OnHealthChanged;
        public event Action OnDeath;

        #region UnityEvents

        private void Awake()
        {
            SceneDataHandler.Loaders.Add(this);
            SceneDataHandler.Savers.Add(this);
        }

        private void Start()
        {
            if (_currentHealth <= 0)
            {
                RestoreHealth();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == 9)
                return;

            if (!_isIndestructible)
            {
                float collisionSpeed = collision.relativeVelocity.magnitude;
                int damage = Mathf.RoundToInt(_damageConstant + collisionSpeed * _damageMultiplier);

                TryChangeHealthAmount(-Math.Abs(damage));
            }
        }
        #endregion

        public void SetCurrentHealth(int health)
        {
            if (health > 0)
            {
                _currentHealth = health;

                OnHealthChanged?.Invoke();
            }
        }

        public void SetMaxHealth(int health)
        {
            if (health > 0)
            {
                _maxHealth = health;
            }
        }

        public void RestoreHealth()
        {
            _currentHealth = _maxHealth;

            OnHealthChanged?.Invoke();
        }

        public void TryChangeHealthAmount(int amount)
        {
            if (amount != 0)
            {
                _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);

                if (_currentHealth == 0)
                {
                    OnDeath?.Invoke();
                }

                OnHealthChanged?.Invoke();
            }
        }

        public void Load()
        {
            _currentHealth = SceneDataHandler.Data.Health;

            if (_currentHealth <= 0)
            {
                RestoreHealth();
            }

            OnHealthChanged?.Invoke();
        }

        public void Save()
        {
            SceneDataHandler.Data.Health = _currentHealth;
        }
    }
}