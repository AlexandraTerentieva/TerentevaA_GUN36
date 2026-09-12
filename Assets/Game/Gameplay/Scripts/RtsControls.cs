using UnityEngine;
using UnityEngine.InputSystem;
using SampleProject;
using Game.GameEngine.Ecs;
using SampleProject.ResourceObject;

public class RtsControls : MonoBehaviour
{
    [SerializeField] private CommandController commandController; // Ссылка на контроллер команд (чтобы отправлять приказы)
    private Camera mainCamera; // Главная камера для луча

    private void Start()
    {
        mainCamera = Camera.main; // Находим камеру на сцене
    }

    private void Update()
    {
        // Если нажата левая кнопка мыши
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue(); // Позиция курсора на экране
            Ray ray = mainCamera.ScreenPointToRay(mousePos); // Строим луч от камеры через курсор

            // Пускаем луч и смотрим, во что он попал
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                string tag = hit.collider.tag; // Узнаём тег объекта

                // ЕСЛИ ЭТО ЗЕМЛЯ — идём в точку
                if (tag == "Ground")
                {
                    GameObject tempPoint = new GameObject("TempPoint");
                    tempPoint.transform.position = hit.point;
                    commandController.MoveToPosition(tempPoint.transform);
                    Destroy(tempPoint);
                }
                // ЕСЛИ ЭТО ВРАГ — атакуем
                else if (tag == "Enemy")
                {
                    Entity entity = hit.collider.GetComponent<Entity>();
                    if (entity != null)
                    {
                        commandController.AttackTarget(entity);
                    }
                }
                // ЕСЛИ ЭТО РЕСУРС — собираем
                else if (tag == "Resource")
                {
                    // Находим на объекте скрипт ResourceEntity (в нём хранятся данные ресурса)
                    ResourceEntity resourceEntity = hit.collider.GetComponent<ResourceEntity>();
                    if (resourceEntity != null)
                    {
                        // Отправляем команду на сбор ресурса
                        commandController.GatherResource(resourceEntity);

                        // находим скрипт ResourceCollector на персонаже (чтобы увеличить счётчик ресурсов)
                        ResourceCollector collector = FindObjectOfType<ResourceCollector>();
                        if (collector != null)
                        {
                            collector.OnResourceCollected(); // Вызываем метод увеличения счётчика
                        }
                    }
                }
            }
        }
    }
}