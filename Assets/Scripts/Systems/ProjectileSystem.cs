using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using UnityEngine;
using Zenject;

namespace Netologia.Systems
{
    public class ProjectileSystem : GameObjectPoolContainer<Projectile>, Director.IManualUpdate
    {
        private EffectSystem _effects;      //injected

        [SerializeField, Min(0.01f)]
        private float _hitDistance = 0.3f;

        public void ManualUpdate()
        {
            foreach (var pair in this)
            {
                foreach (var projectile in pair)
                {
                    if (projectile == null || !projectile.gameObject.activeSelf) continue;

                    Vector3 targetPos = projectile.TargetPosition;

                    projectile.transform.position = Vector3.MoveTowards(
                        projectile.transform.position,
                        targetPos,
                        projectile.MoveSpeed * Time.deltaTime
                    );

                    float distance = Vector3.Distance(projectile.transform.position, targetPos);
                    if (distance < _hitDistance)
                    {
                        projectile.DealDamage();

                        if (projectile.HasEffect && projectile.HitEffect != null)
                        {
                            var effect = _effects[projectile.HitEffect].Get;
                            effect.transform.position = projectile.transform.position;
                            effect.Play();
                        }

                        if (projectile.HasSound && projectile.HitSound != null)
                        {
                            AudioManager.PlayHit(projectile.HitSound);
                        }

                        this[projectile.Ref].ReturnElement(projectile.ID);
                    }
                }
            }
        }

        public void OnDespawnUnit(int unitID)
        {
            foreach (var pool in this)
                foreach (var projectile in pool)
                    if (projectile.TargetID == unitID)
                        projectile.ResetTarget();
        }

        [Inject]
        private void Construct(EffectSystem effects)
        {
            (_effects) = (effects);
            _hitDistance *= _hitDistance;
        }
    }
}