using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using Zenject;
using UnityEngine;

namespace Netologia.Systems
{
    /// <summary>
    /// Управляет всеми башнями на сцене.
    /// Отвечает за поиск целей, стрельбу и перезарядку.
    /// </summary>
    public class TowerSystem : GameObjectPoolContainer<Tower>, Director.IManualUpdate
    {
        // Системы, которые нужны для работы
        private UnitSystem _units;              // Чтобы искать врагов
        private ProjectileSystem _projectiles;  // Чтобы создавать снаряды

        /// <summary>
        /// Вызывается каждый кадр из Director.
        /// Обновляет состояние всех башен.
        /// </summary>
        public void ManualUpdate()
        {
            // Проходим по всем башням
            foreach (var pair in this)
            {
                foreach (var tower in pair)
                {
                    if (tower == null || !tower.gameObject.activeSelf) continue;

                    // Проверяем, не перезаряжается ли башня
                    if (!tower.DecrementAttackReload(Time.deltaTime)) continue;

                    // Если у башни нет цели, или цель умерла/вышла из радиуса — ищем новую
                    if (tower.Target == null || !tower.Target.gameObject.activeSelf || !IsTargetInRange(tower))
                    {
                        tower.Target = FindTargetInRange(tower);
                        if (tower.Target == null) continue;
                    }

                    // Если цель есть — стреляем
                    Shoot(tower);
                }
            }
        }

        /// <summary>
        /// Проверяет, находится ли текущая цель башни в радиусе.
        /// </summary>
        private bool IsTargetInRange(Tower tower)
        {
            if (tower.Target == null) return false;
            float distance = Vector3.Distance(tower.transform.position, tower.Target.transform.position);
            return distance <= tower.Range;
        }

        /// <summary>
        /// Ищет ближайшего врага, который находится строго в радиусе башни.
        /// Если врагов в радиусе нет — возвращает null.
        /// </summary>
        private Unit FindTargetInRange(Tower tower)
        {
            float range = tower.Range;
            Vector3 position = tower.transform.position;
            Unit closest = null;
            float closestDist = float.MaxValue;

            // Перебираем всех активных врагов
            foreach (var pair in _units)
            {
                foreach (var unit in pair)
                {
                    if (unit == null || !unit.gameObject.activeSelf) continue;

                    float dist = Vector3.Distance(unit.transform.position, position);

                    // Враг должен быть строго в радиусе
                    if (dist <= range && dist < closestDist)
                    {
                        closestDist = dist;
                        closest = unit;
                    }
                }
            }

            return closest;
        }

        /// <summary>
        /// Создаёт снаряд и отправляет его в цель.
        /// Если у башни нет снаряда (Projectile == null) — выходим без ошибки.
        /// Это нужно для казармы, которая не стреляет, но является башней.
        /// </summary>
        private void Shoot(Tower tower)
        {
            // ✅ ЗАЩИТА ОТ ОШИБКИ: если снаряд отсутствует — пропускаем выстрел
            if (tower == null || tower.Projectile == null) return;

            // Получаем снаряд из пула
            Projectile projectile = _projectiles[tower.Projectile].Get;

            // Настраиваем снаряд
            projectile.PrepareData(
                tower.transform.position,  // Позиция выстрела
                tower.Target,              // Цель
                tower.Damage,              // Урон
                tower.AttackElemental      // Тип урона (физический, огонь, лёд)
            );

            // Вызываем событие атаки у башни (эффекты, звуки)
            tower.Attack();
        }

        /// <summary>
        /// Когда враг умирает — сбрасываем цель у башен, которые на него целились.
        /// </summary>
        public void OnDespawnUnit(int unitID)
        {
            foreach (var pair in this)
                foreach (var tower in pair)
                    if (tower.TargetID == unitID)
                        tower.Target = null;
        }

        /// <summary>
        /// Zenject-инъекция зависимостей.
        /// </summary>
        [Inject]
        private void Construct(UnitSystem units, ProjectileSystem projectiles)
        {
            _units = units;
            _projectiles = projectiles;
        }
    }
}