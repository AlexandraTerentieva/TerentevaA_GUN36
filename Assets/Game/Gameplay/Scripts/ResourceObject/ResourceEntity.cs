using Game.GameEngine.Ecs;
using UnityEngine;

namespace SampleProject.ResourceObject
{
    public sealed class ResourceEntity : Entity
    {
        protected override void Init()
        {
            this.SetData(new TransformComponent
            {
                value = this.transform,
                radius = 1f
            });

            // количество ресурсов (50 золота)
            this.SetData(new ResourceComponent
            {
                amount = 50,
                type = ResourceType.Gold
            });
        }
    }

    // типы ресурсов
    public enum ResourceType
    {
        Gold,   // золото
        Wood,   // дерево
        Stone   // камень
    }

    // структура для хранения ресурсов в ECS
    public struct ResourceComponent
    {
        public int amount;        // сколько осталось
        public ResourceType type; // какой тип
    }
}