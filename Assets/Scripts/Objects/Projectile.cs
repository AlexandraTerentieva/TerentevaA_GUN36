using Netologia.Behaviours;
using UnityEngine;
using Netologia.TowerDefence.Behaviors; // ← ДОБАВЛЕНО

namespace Netologia.TowerDefence
{
    /// <summary>
    /// Снаряд. Летит в цель, наносит урон, начисляет золото.
    /// </summary>
    public class Projectile : MonoBehaviour, IPoolElement<Projectile>
    {
        // ============================================================
        //  ПРИВАТНЫЕ ПОЛЯ
        // ============================================================

        private float _damage;
        private Vector3? _endPosition;
        private Unit _target;
        private ElementalType _elementalType;

        // ============================================================
        //  ПУБЛИЧНЫЕ ПОЛЯ
        // ============================================================

        [field: SerializeField]
        public ParticleSystem HitEffect { get; private set; }

        [field: SerializeField]
        public AudioClip HitSound { get; private set; }

        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        public bool HasEffect { get; private set; }
        public bool HasSound { get; private set; }

        public Projectile Ref { get; set; }
        public int ID { get; set; }

        public Vector3 TargetPosition => _endPosition ?? _target.transform.position;
        public int TargetID { get; private set; } = -1;

        // ============================================================
        //  МЕТОДЫ
        // ============================================================

        public void DealDamage()
        {
            if (_endPosition.HasValue) return;
            if (_target == null) return;

            Debug.Log($"Снаряд нанёс {_damage} урона врагу {_target.name}");

            // Отнимаем здоровье
            _target.CurrentHealth -= _damage;
            Debug.Log($"Здоровье врага стало: {_target.CurrentHealth}");

            // Если враг умер — начисляем золото
            if (_target.CurrentHealth <= 0)
            {
                Debug.Log("Враг УМЕР!");

                // Начисляем золото
                if (Director.Instance != null)
                {
                    Director.Instance.AddMoney(_target.Stats.Cost);
                    Debug.Log($"Золото +{_target.Stats.Cost}");
                }
                else
                {
                    Debug.LogWarning("Director.Instance == null! Золото не начислено.");
                }

                // Удаляем врага
                Destroy(_target.gameObject);
            }

            // Эффект (огонь/лёд)
            _target.TryAddEffect(TimeManager.Time, _elementalType);
        }

        public void ResetTarget()
            => (_endPosition, _target) = (_target.transform.position, null);

        public void PrepareData(Vector3 position, Unit target, float damage, ElementalType type)
        {
            transform.position = position;
            (_target, _damage, _elementalType, _endPosition) = (target, damage, type, null);
            TargetID = target.ID;
        }

        private void Awake()
        {
            HasEffect = HitEffect != null;
            HasSound = HitSound != null;
        }
    }
}