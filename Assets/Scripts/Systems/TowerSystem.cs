using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using Zenject;
using UnityEngine;

namespace Netologia.Systems
{
    public class TowerSystem : GameObjectPoolContainer<Tower>, Director.IManualUpdate
    {
        private UnitSystem _units;
        private ProjectileSystem _projectiles;

        public void ManualUpdate()
        {
            foreach (var pair in this)
            {
                foreach (var tower in pair)
                {
                    if (tower == null || !tower.gameObject.activeSelf) continue;

                    // Проверяем перезарядку
                    if (!tower.DecrementAttackReload(Time.deltaTime)) continue;

                    // КАЖДЫЙ КАДР ИЩЕМ ЦЕЛЬ ЗАНОВО, ИГНОРИРУЯ СТАРУЮ
                    tower.Target = FindClosestEnemyInRange(tower);
                    if (tower.Target == null) continue;

                    Shoot(tower);
                }
            }
        }

        private Unit FindClosestEnemyInRange(Tower tower)
        {
            float range = tower.Range;
            Vector3 position = tower.transform.position;
            Unit closest = null;
            float closestDist = range;

            Unit[] allUnits = FindObjectsOfType<Unit>();

            foreach (Unit unit in allUnits)
            {
                if (unit == null || !unit.gameObject.activeSelf) continue;

                float dist = Vector3.Distance(unit.transform.position, position);

                if (dist <= range && dist < closestDist)
                {
                    closestDist = dist;
                    closest = unit;
                }
            }

            return closest;
        }

        private void Shoot(Tower tower)
        {
            Projectile projectile = _projectiles[tower.Projectile].Get;
            projectile.PrepareData(
                tower.transform.position,
                tower.Target,
                tower.Damage,
                tower.AttackElemental
            );
            tower.Attack();
        }

        public void OnDespawnUnit(int unitID)
        {
            foreach (var pair in this)
                foreach (var tower in pair)
                    if (tower.TargetID == unitID)
                        tower.Target = null;
        }

        [Inject]
        private void Construct(UnitSystem units, ProjectileSystem projectiles)
        {
            _units = units;
            _projectiles = projectiles;
        }
    }
}