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
        private Director _director;
        private EffectSystem _effects;
        private Constants _constants;
        private Vector3[] _path;

        [SerializeField, Min(0.01f)]
        private float _arrivalDistance = 0.1f;

        public event Action<int> OnDespawnUnitHandler;

        public Unit FindTarget(in Vector3 position, float range)
        {
            float rangeSquared = range * range;
            var target = default(Unit);

            foreach (var pair in this)
            {
                foreach (var unit in pair)
                {
                    if (unit == null || !unit.gameObject.activeSelf) continue;

                    var distance = Vector3.SqrMagnitude(unit.transform.position - position);

                    if (distance > rangeSquared) continue;

                    if (distance < rangeSquared)
                    {
                        rangeSquared = distance;
                        target = unit;
                    }
                }
            }

            return target;
        }

        public void ManualUpdate()
        {
            foreach (var pair in this)
            {
                foreach (var unit in pair)
                {
                    if (unit == null || !unit.gameObject.activeSelf) continue;

                    if (unit.CurrentHealth <= 0)
                    {
                        DespawnUnit(unit, unit.transform.position);
                        continue;
                    }

                    Vector3[] path = _path;
                    if (path == null || path.Length == 0) continue;

                    int index = unit.PathIndex;
                    if (index >= path.Length) continue;

                    Vector3 targetPos = path[index];
                    float speed = unit.MoveSpeed * Time.deltaTime;
                    unit.transform.position = Vector3.MoveTowards(
                        unit.transform.position,
                        targetPos,
                        speed
                    );

                    float distance = Vector3.Distance(unit.transform.position, targetPos);
                    if (distance < _arrivalDistance)
                    {
                        unit.PathIndex++;
                        if (unit.PathIndex >= path.Length)
                        {
                            _director.AddPlayerDamage(1);
                            DespawnUnit(unit, unit.transform.position);
                        }
                    }
                }
            }
        }

        private void DespawnUnit(Unit unit, in Vector3 position)
        {
            if (unit.HasEffect)
            {
                var effect = _effects[unit.DieEffect].Get;
                effect.transform.position = position;
                effect.Play();
            }
            if (unit.HasSound)
            {
                AudioManager.PlayHit(unit.DieSound);
            }

            _director.AddMoney(unit.Stats.Cost);
            this[unit.Ref].ReturnElement(unit.ID);
        }

        [Inject]
        private void Construct(EffectSystem effects, Director director, Constants constants, WaveController path)
        {
            (_effects, _director, _constants, _path) = (effects, director, constants, path.GetPath());
            _arrivalDistance *= _arrivalDistance;
            AwakeMethod = t => t.Constants = _constants;
        }
    }
}