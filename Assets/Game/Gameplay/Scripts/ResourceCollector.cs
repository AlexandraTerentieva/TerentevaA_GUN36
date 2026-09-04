using UnityEngine;
using Game.GameEngine.Ecs;
using SampleProject;

public class ResourceCollector : MonoBehaviour
{
    private Entity entity;           // Ссылка на Entity юнита
    private int collected = 0;       // Счётчик собранных ресурсов
    private int maxBeforeReturn = 5; // После скольки идём на базу

    private Transform baseTransform; // Позиция базы

    private void Start()
    {
        entity = GetComponent<Entity>(); // Находим Entity юнита
        baseTransform = GameObject.FindGameObjectWithTag("Base").transform; // Ищем базу по тегу
    }

    // Этот метод вызывается, когда юнит собрал ресурс
    public void OnResourceCollected()
    {
        collected++; // Увеличиваем счётчик

        if (collected >= maxBeforeReturn)
        {
            // Отправляем юнита к базе
            CommandController controller = FindObjectOfType<CommandController>();
            controller.MoveToPosition(baseTransform);

            // Сбрасываем счётчик (после сдачи)
            collected = 0;
        }
    }
}