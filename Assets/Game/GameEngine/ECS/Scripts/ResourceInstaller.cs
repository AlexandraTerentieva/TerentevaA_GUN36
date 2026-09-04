using GameECS;
using UnityEngine;
using SampleProject.ResourceObject;

namespace Game.GameEngine.Ecs
{
    [CreateAssetMenu(
        fileName = "New Installer «Resource»",
        menuName = "Game/GameEngine/Ecs/New Installer «Resource»"
    )]
    public sealed class ResourceInstaller : EcsInstaller
    {
        public override void Install(EcsWorld world)
        {
            // Регистрируем ResourceComponent в ECS, чтобы система могла его использовать
            world.DeclareComponent<ResourceComponent>();
        }
    }
}