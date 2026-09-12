using System.Linq;
using Game.GameEngine.Ecs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleProject
{
    public sealed class CommandController : MonoBehaviour
    {
        [SerializeField]
        private Entity entity;

        // Проверяет, жив ли Entity игрока (через GameObject, а не через ECS)
        // Это защищает от ошибки IndexOutOfRangeException при смерти игрока
        private bool IsEntityAlive()
        {
            if (entity == null) return false;
            return entity.gameObject != null && entity.gameObject.activeInHierarchy;
        }

        // Отправляет команду "Идти в точку" (для игрока)
        [Button]
        public void MoveToPosition(Transform point)
        {
            if (!IsEntityAlive()) return; // Проверяем, жив ли игрок

            this.entity.SetData(new CommandRequest
            {
                type = CommandType.MOVE_TO_POSITION,
                args = point.position,
                status = CommandStatus.IDLE
            });
        }

        // Отправляет команду "Атаковать цель" (для игрока)
        // С проверкой: если цель уничтожена — атака не отправляется
        [Button]
        public void AttackTarget(Entity target)
        {
            if (!IsEntityAlive()) return; // Проверяем, жив ли игрок

            // Проверяем, жив ли GameObject цели (безопасно, без ошибок ECS)
            if (target == null || target.gameObject == null || !target.gameObject.activeInHierarchy)
            {
                return;
            }

            this.entity.SetData(new CommandRequest
            {
                type = CommandType.ATTACK_TARGET,
                args = target,
                status = CommandStatus.IDLE
            });
        }

        // Отправляет команду "Собрать ресурс" (для игрока)
        [Button]
        public void GatherResource(Entity resource)
        {
            if (!IsEntityAlive()) return; // Проверяем, жив ли игрок

            this.entity.SetData(new CommandRequest
            {
                type = CommandType.GATHER_RESOURCE,
                args = resource,
                status = CommandStatus.IDLE
            });
        }

        // Отправляет команду "Патрулировать по точкам"
        [Button]
        public void Patrol(Transform[] points)
        {
            if (!IsEntityAlive()) return; // Проверяем, жив ли игрок

            this.entity.SetData(new CommandRequest
            {
                type = CommandType.PATROL_BY_POINTS,
                args = points.Select(it => it.position).ToList(),
                status = CommandStatus.IDLE
            });
        }

        // Останавливает текущую команду
        [Button]
        public void Stop()
        {
            if (!IsEntityAlive()) return; // Проверяем, жив ли игрок

            this.entity.RemoveData<CommandRequest>();
        }

        // Отправляет команду "Идти в точку" любому юниту (для группы)
        // Используется в SelectionManager
        [Button]
        public void MoveToPosition(Entity targetEntity, Transform point)
        {
            if (targetEntity == null) return; // Проверяем, существует ли юнит

            targetEntity.SetData(new CommandRequest
            {
                type = CommandType.MOVE_TO_POSITION,
                args = point.position,
                status = CommandStatus.IDLE
            });
        }

        // Отправляет команду "Атаковать цель" любому юниту (для группы)
        // С проверкой: если цель уничтожена — атака не отправляется
        [Button]
        public void AttackTarget(Entity targetEntity, Entity target)
        {
            if (targetEntity == null) return; // Проверяем, существует ли юнит

            // Проверяем, жив ли GameObject цели (безопасно, без ошибок ECS)
            if (target == null || target.gameObject == null || !target.gameObject.activeInHierarchy)
            {
                return;
            }

            targetEntity.SetData(new CommandRequest
            {
                type = CommandType.ATTACK_TARGET,
                args = target,
                status = CommandStatus.IDLE
            });
        }

        // Отправляет команду "Собрать ресурс" любому юниту (для группы)
        [Button]
        public void GatherResource(Entity targetEntity, Entity resource)
        {
            if (targetEntity == null) return; // Проверяем, существует ли юнит

            targetEntity.SetData(new CommandRequest
            {
                type = CommandType.GATHER_RESOURCE,
                args = resource,
                status = CommandStatus.IDLE
            });
        }
    }
}