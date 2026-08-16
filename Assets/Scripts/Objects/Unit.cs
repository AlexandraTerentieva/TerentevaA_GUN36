using System.Collections.Generic;
using Netologia.Behaviours;
using Netologia.TowerDefence.Behaviors;
using Netologia.TowerDefence.Settings;
using UnityEngine;

namespace Netologia.TowerDefence
{
    /// <summary>
    /// Враг. Содержит здоровье, эффекты, урон и смерть.
    /// Комментарии для себя — чтобы не забыть, что тут происходит.
    /// </summary>
    public class Unit : MonoBehaviour, IPoolElement<Unit>
    {
        // ============================================================
        //  НАСТРОЙКИ В ИНСПЕКТОРЕ
        // ============================================================

        [field: SerializeField]
        public ParticleSystem DieEffect { get; private set; }   // Эффект при смерти (искры, дым)

        [field: SerializeField]
        public AudioClip DieSound { get; private set; }         // Звук при смерти

        // ============================================================
        //  ХАРАКТЕРИСТИКИ ВРАГА (берутся из пресета)
        // ============================================================

        public UnitPresetSettings.Stats Stats { get; private set; }  // Здоровье, скорость, награда
        public bool HasEffect { get; private set; }                  // Есть ли эффект смерти?
        public bool HasSound { get; private set; }                   // Есть ли звук смерти?
        public Constants Constants { get; set; }                     // Общие настройки игры

        public int PathIndex { get; set; }          // Текущая точка пути (для движения)
        public float CurrentHealth { get; set; }    // Текущее здоровье (меняется от урона)

        public Unit Ref { get; set; }               // Ссылка на себя (для пула)
        public int ID { get; set; }                 // ID в пуле

        public UnitVisual Visual { get; private set; }  // Визуал (анимации)

        /// <summary>
        /// Скорость движения. Если есть эффект льда — замедляется.
        /// </summary>
        public float MoveSpeed
            => _iceEffects.Count > 0
                ? Mathf.Pow(Constants.IceDebuffMoveSpeedMult, _iceEffects.Count) * Stats.MoveSpeed
                : Stats.MoveSpeed;

        // ============================================================
        //  ЭФФЕКТЫ (огонь, лёд)
        // ============================================================

        private List<float> _fireEffects;   // Время действия огня
        private List<float> _iceEffects;    // Время действия льда

        /// <summary>
        /// Сколько эффектов определённого типа сейчас активно
        /// </summary>
        public int CountEffect(ElementalType type)
            => type switch
            {
                ElementalType.Fire => _fireEffects.Count,
                ElementalType.Ice => _iceEffects.Count,
                _ => 0
            };

        /// <summary>
        /// Добавляет эффект (огонь или лёд) на время
        /// </summary>
        public void TryAddEffect(float time, ElementalType type)
        {
            var list = default(List<float>);
            var max = default(int);
            switch (type)
            {
                case ElementalType.Fire:
                    (list, max) = (_fireEffects, Constants.FireDebuffMaxStack);
                    break;
                case ElementalType.Ice:
                    (list, max) = (_iceEffects, Constants.IceDebuffMaxStack);
                    break;
                default:
                    return;
            }

            // Если эффектов больше максимума — заменяем самый старый
            if (list.Count >= max)
                list[^1] = time;
            else
                list.Add(time);

            list.Sort(Compare);
        }

        /// <summary>
        /// Убирает эффект, если его время вышло
        /// </summary>
        public void TryRemoveEffect(float time, ElementalType type)
        {
            var list = default(List<float>);
            var delay = default(float);
            (list, delay) = type switch
            {
                ElementalType.Fire => (_fireEffects, Constants.FireDebuffDuration),
                ElementalType.Ice => (_iceEffects, Constants.IceDebuffDuration),
                _ => (null, 0)
            };

            if (list.Count == 0) return;
            if (time - list[^1] >= delay) list.RemoveAt(list.Count - 1);
        }

        // ============================================================
        //  ВОЗРОЖДЕНИЕ ИЗ ПУЛА
        // ============================================================

        /// <summary>
        /// Создаёт врага (или возрождает из пула)
        /// </summary>
        public void Respawn(UnitPresetSettings.Stats stats, Vector3 position)
        {
            (Stats, CurrentHealth, PathIndex) = (stats, stats.Health, 0);
            transform.position = position;

            // Первое появление — настраиваем эффекты
            if (_fireEffects is null)
            {
                HasEffect = DieEffect != null;
                HasSound = DieSound != null;
                Visual = GetComponent<UnitVisual>();

                _fireEffects = new List<float>(Constants.FireDebuffMaxStack);
                _iceEffects = new List<float>(Constants.IceDebuffMaxStack);
            }

            _fireEffects.Clear();
            _iceEffects.Clear();
        }

        // ============================================================
        //  УРОН И СМЕРТЬ (самое важное)
        // ============================================================

        /// <summary>
        /// Получает урон. Если здоровье упало до 0 — УМИРАЕТ.
        /// </summary>
        public void TakeDamage(float damage)
        {
            // Отнимаем здоровье
            CurrentHealth -= damage;

            // Пишем в консоль, чтобы видеть, что урон проходит
            Debug.Log($"Урон {damage}. Осталось здоровья: {CurrentHealth}");

            // Если здоровье кончилось — враг умирает
            if (CurrentHealth <= 0)
            {
                Debug.Log("Враг УМЕР!");
                Destroy(gameObject);
            }
        }

        // ============================================================
        //  ВСПОМОГАТЕЛЬНОЕ
        // ============================================================

        /// <summary>
        /// Сортировка эффектов по времени (самые старые — в конце)
        /// </summary>
        private static int Compare(float a, float b)
            => a > b ? -1 : 1;
    }
}