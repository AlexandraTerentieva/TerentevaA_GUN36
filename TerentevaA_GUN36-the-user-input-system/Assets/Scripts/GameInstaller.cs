using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Создаём экземпляр контроллера ввода и включаем его
        GameControls controls = new GameControls();
        controls.Enable();

        // Регистрируем сам объект Controls в контейнере как синглтон
        Container.Bind<GameControls>().FromInstance(controls).AsSingle();

        // Регистрируем карту действий "Game" (где лежит Restart)
        Container.Bind<GameControls.GameActions>().FromInstance(controls.Game).AsSingle();
    }
}