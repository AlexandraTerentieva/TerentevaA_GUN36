using System;
using Behaviours;
using JetBrains.Annotations;
using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using Netologia.TowerDefence.Settings;
using UnityEngine;
using Zenject;

namespace Netologia.Systems
{
    public class UnitSystem : GameObjectPoolContainer<Unit>, Director.IManualUpdate
    {
        // ============================================================
        //  ЗАВИСИМОСТИ (внедряются через Zenject)
        // ============================================================

        private Director _director;          // Главный контроллер (для урона и золота)
        private EffectSystem _effects;       // Система эффектов (для смерти)
        private Constants _constants;        // Константы игры
        private Vector3[] _path;             // Путь, по которому идут враги

        // ============================================================
        //  НАСТРОЙКИ В ИНСПЕКТОРЕ
        // ============================================================

        [SerializeField, Min(0.01f)]
        private float _arrivalDistance = 0.1f; // Дистанция, на которой враг считается достигшим точки

        public event Action<int> OnDespawnUnitHandler; // Событие при удалении врага

        // ============================================================
        //  ПОИСК ЦЕЛИ ДЛЯ БАШЕН
        // ============================================================

        /// <summary>
        /// Ищет ближайшего врага в радиусе range от позиции position.
        /// Используется башнями для выбора цели.
        /// </summary>
        [CanBeNull]
        public Unit FindTarget(in Vector3 position, float range)
        {
            range *= range;
            var target = default(Unit);
            foreach (var pair in this)
            {
                foreach (var unit in pair)
                {
                    var distance = Vector3.SqrMagnitude(unit.transform.position - position);
                    if (distance < range)
                        (range, target) = (distance, unit);
                }
            }
            return target;
        }

        // ============================================================
        //  ДВИЖЕНИЕ ВРАГОВ ПО ПУТИ
        // ============================================================

        /// <summary>
        /// Вызывается каждый кадр из Director.
        /// Двигает врагов по пути, проверяет конец пути.
        /// </summary>
        public void ManualUpdate()
        {
            foreach (var pair in this)
            {
                foreach (var unit in pair)
                {
                    if (unit == null || !unit.gameObject.activeSelf) continue;

                    // Если путь не задан — пропускаем
                    Vector3[] path = _path;
                    if (path == null || path.Length == 0) continue;

                    // Индекс текущей точки
                    int index = unit.PathIndex;
                    if (index >= path.Length) continue;

                    // Точка, к которой движемся
                    Vector3 targetPos = path[index];

                    // Движение к точке
                    float speed = unit.MoveSpeed * Time.deltaTime;
                    unit.transform.position = Vector3.MoveTowards(
                        unit.transform.position,
                        targetPos,
                        speed
                    );

                    // Проверка: достиг ли враг точки
                    float distance = Vector3.Distance(unit.transform.position, targetPos);
                    if (distance < _arrivalDistance)
                    {
                        unit.PathIndex++;

                        // Если враг дошёл до конца пути
                        if (unit.PathIndex >= path.Length)
                        {
                            // ============================================================
                            // ВРАГ ДОШЁЛ ДО КОНЦА → НАНОСИМ УРОН ИГРОКУ
                            // БЕЗ ЗОЛОТА!
                            // ============================================================
                            _director.AddPlayerDamage(1);          // Урон игроку
                            this[unit.Ref].ReturnElement(unit.ID); // Удаляем врага
                            // ============================================================
                        }
                    }
                }
            }
        }

        // ============================================================
        //  СМЕРТЬ ВРАГА ОТ БАШНИ (с золотом)
        // ============================================================

        /// <summary>
        /// Удаляет врага при смерти от башни.
        /// Начисляет золото, эффекты и звуки.
        /// </summary>
        private void DespawnUnit(Unit unit, in Vector3 position)
        {
            // Эффект смерти
            if (unit.HasEffect)
            {
                var effect = _effects[unit.DieEffect].Get;
                effect.transform.position = position;
                effect.Play();
            }

            // Звук смерти
            if (unit.HasSound)
            {
                AudioManager.PlayHit(unit.DieSound);
            }

            // Начисляем золото
            _director.AddMoney(unit.Stats.Cost);

            // Возвращаем врага в пул
            this[unit.Ref].ReturnElement(unit.ID);
        }

        // ============================================================
        //  УСТАНОВКА ПУТИ (из PathCollector)
        // ============================================================

        /// <summary>
        /// Устанавливает путь для врагов.
        /// Вызывается из PathCollector.
        /// </summary>
        public void SetPath(Vector3[] path)
        {
            _path = path;
            Debug.Log($"UnitSystem: путь установлен ({path.Length} точек)");
        }

        // ============================================================
        //  КОНСТРУКТОР (внедрение зависимостей через Zenject)
        // ============================================================

        [Inject]
        private void Construct(EffectSystem effects, Director director, Constants constants, WaveController path)
        {
            (_effects, _director, _constants, _path) = (effects, director, constants, path.GetPath());
            _arrivalDistance *= _arrivalDistance;
            AwakeMethod = t => t.Constants = _constants;
        }
    }
}