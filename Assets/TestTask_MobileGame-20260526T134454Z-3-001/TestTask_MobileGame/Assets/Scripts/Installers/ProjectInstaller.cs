using Core.InputSource;
using Core.SaveLoad;
using Core.Utils;
using InputType;
using Models;
using UnityEngine;
using Zenject;

namespace Installers
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        public override void Start() => Application.targetFrameRate = NumericConstants.FPS;

        public override void InstallBindings()
        {
            Container
                .Bind<ISaveLoadDataHandler>()
                .To<PlayerPrefsSaveLoadDataHandler>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<GameScoreModel>()
                .AsSingle();
            Container
                .BindInterfacesAndSelfTo<GameInputProcessor>()
                .AsSingle()
                .WithArguments(new MouseInput())
                .Lazy();
        }
    }
}