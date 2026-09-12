using UnityEngine;
using System.Collections.Generic;
using Game.GameEngine.Ecs;
using SampleProject;
using SampleProject.ResourceObject;

public class SelectionManager : MonoBehaviour
{
    // --- ОБЪЯВЛЯЕМ ПЕРЕМЕННЫЕ ---
    private Camera mainCamera;                  // Главная камера (для расчётов)
    private Vector3 startMousePos;             // Позиция мыши в начале выделения
    private Vector3 endMousePos;               // Позиция мыши в конце выделения
    private bool isSelecting = false;          // Идёт ли процесс выделения?

    private List<Entity> selectedUnits = new List<Entity>();       // Список выделенных Entity (для команд)
    private List<GameObject> selectedObjects = new List<GameObject>(); // Список выделенных объектов (для подсветки)

    [SerializeField] private LayerMask groundLayer; // Слой земли (чтобы клик правой кнопкой работал только по земле)

    // Словарь для хранения оригинальных цветов юнитов (чтобы вернуть при снятии выделения)
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

    // --- МЕТОД START ВЫЗЫВАЕТСЯ ПРИ ЗАПУСКЕ ---
    private void Start()
    {
        mainCamera = Camera.main; // Находим камеру
    }

    // --- МЕТОД UPDATE ВЫЗЫВАЕТСЯ КАЖДЫЙ КАДР ---
    private void Update()
    {
        // --- НАЧАЛО ВЫДЕЛЕНИЯ (ЗАЖАЛИ ЛЕВУЮ КНОПКУ МЫШИ) ---
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition; // Запоминаем, где начали
            isSelecting = true;                 // Включаем режим выделения
            ClearSelection();                   // Снимаем предыдущую подсветку
        }

        // --- ВО ВРЕМЯ ВЫДЕЛЕНИЯ (ТЯНЕМ РАМКУ) ---
        if (isSelecting)
        {
            endMousePos = Input.mousePosition; // Обновляем позицию рамки
        }

        // --- ОКОНЧАНИЕ ВЫДЕЛЕНИЯ (ОТПУСТИЛИ ЛЕВУЮ КНОПКУ) ---
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;    // Выключаем режим выделения
            SelectUnits();          // Вызываем метод выделения юнитов
        }

        // --- ПРАВАЯ КНОПКА МЫШИ (выполнить действие) ---
        if (Input.GetMouseButtonDown(1))
        {
            PerformAction(); // атака, сбор или движение
        }
    }

    // --- РИСУЕМ РАМКУ ВЫДЕЛЕНИЯ (НА ЭКРАНЕ) ---
    private void OnGUI()
    {
        if (isSelecting) // Если идёт выделение
        {
            // Получаем прямоугольник рамки
            Rect rect = GetScreenRect(startMousePos, endMousePos);

            // Рисуем заливку (полупрозрачный синий)
            DrawScreenRect(rect, new Color(0.2f, 0.6f, 1f, 0.1f));

            // Рисуем границу (синяя)
            DrawScreenRectBorder(rect, 2, new Color(0.2f, 0.6f, 1f, 0.8f));
        }
    }

    // --- ПОЛУЧАЕМ ПРЯМОУГОЛЬНИК ВЫДЕЛЕНИЯ (ИЗ ДВУХ ТОЧЕК) ---
    private Rect GetScreenRect(Vector3 start, Vector3 end)
    {
        float x = Mathf.Min(start.x, end.x);
        float y = Mathf.Min(Screen.height - start.y, Screen.height - end.y);
        float width = Mathf.Abs(start.x - end.x);
        float height = Mathf.Abs(start.y - end.y);
        return new Rect(x, y, width, height);
    }

    // --- РИСУЕМ ЗАЛИВКУ РАМКИ ---
    private void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    // --- РИСУЕМ ГРАНИЦУ РАМКИ (4 ЛИНИИ) ---
    private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Верхняя линия
        DrawScreenRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
        // Нижняя линия
        DrawScreenRect(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), color);
        // Левая линия
        DrawScreenRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
        // Правая линия
        DrawScreenRect(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), color);
    }

    // --- ВЫДЕЛЯЕМ ВСЕХ СОЮЗНИКОВ (ALLY), ПОПАВШИХ В РАМКУ ---
    private void SelectUnits()
    {
        // Очищаем списки перед новым выделением
        selectedUnits.Clear();
        selectedObjects.Clear();
        ClearSelection(); // Снимаем подсветку со старых юнитов

        // Находим всех союзников на сцене по тегу "Ally"
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        // Проходим по каждому союзнику
        foreach (GameObject ally in allies)
        {
            // Проверяем, попадает ли он в рамку
            if (IsInsideSelectionBox(ally))
            {
                // Получаем Entity союзника
                Entity entity = ally.GetComponent<Entity>();
                if (entity != null)
                {
                    // Добавляем в списки выделенных
                    selectedUnits.Add(entity);
                    selectedObjects.Add(ally);
                    HighlightObject(ally); // Подсвечиваем зелёным
                }
            }
        }
    }

    // --- ПРОВЕРКА, ПОПАДАЕТ ЛИ ОБЪЕКТ В РАМКУ ---
    private bool IsInsideSelectionBox(GameObject obj)
    {
        // Переводим позицию объекта в экранные координаты
        Vector3 screenPos = mainCamera.WorldToScreenPoint(obj.transform.position);

        // Границы рамки
        float minX = Mathf.Min(startMousePos.x, endMousePos.x);
        float maxX = Mathf.Max(startMousePos.x, endMousePos.x);
        float minY = Mathf.Min(startMousePos.y, endMousePos.y);
        float maxY = Mathf.Max(startMousePos.y, endMousePos.y);

        // Проверяем, внутри ли объект
        return screenPos.x >= minX && screenPos.x <= maxX &&
               screenPos.y >= minY && screenPos.y <= maxY;
    }

    // --- ПОДСВЕТКА ВЫДЕЛЕННОГО ЮНИТА (МЕНЯЕМ ЦВЕТ НА ЗЕЛЁНЫЙ) ---
    private void HighlightObject(GameObject obj)
    {
        // Ищем рендерер на объекте (или на дочернем)
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            // Сохраняем оригинальный цвет (если ещё не сохранён)
            if (!originalColors.ContainsKey(obj))
            {
                originalColors[obj] = renderer.material.color;
            }
            // Меняем цвет на зелёный
            renderer.material.color = Color.green;
        }
    }

    // --- СБРОС ПОДСВЕТКИ (ВОЗВРАЩАЕМ ОРИГИНАЛЬНЫЙ ЦВЕТ) ---
    private void ClearSelection()
    {
        // Проходим по всем выделенным объектам
        foreach (GameObject obj in selectedObjects)
        {
            Renderer renderer = obj.GetComponentInChildren<Renderer>();
            if (renderer != null && originalColors.ContainsKey(obj))
            {
                // Возвращаем оригинальный цвет
                renderer.material.color = originalColors[obj];
            }
        }
        // Очищаем список выделенных объектов
        selectedObjects.Clear();
    }

    // ================================================================
    // МОЙ НОВЫЙ МЕТОД: ВЫПОЛНЕНИЕ ДЕЙСТВИЯ (ПРАВАЯ КНОПКА)
    // ================================================================
    // Отправляет выделенных союзников на:
    // - атаку врага (если клик по врагу)
    // - сбор ресурса (если клик по ресурсу)
    // - движение в точку (если клик по земле)
    private void PerformAction()
    {

        // Кидаем луч от камеры через курсор мыши
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            // Находим CommandController на сцене
            CommandController controller = FindObjectOfType<CommandController>();
            if (controller == null)
            {
                return;
            }

            // --- ПРОВЕРКА 1: попали во врага (тег "Enemy") ---
            if (hit.collider.CompareTag("Enemy"))
            {
                // Получаем Entity врага
                Entity targetEntity = hit.collider.GetComponent<Entity>();
                if (targetEntity != null)
                {
                    // Отправляем КАЖДОГО выделенного союзника атаковать этого врага
                    foreach (Entity unit in selectedUnits)
                    {
                        controller.AttackTarget(unit, targetEntity);
                    }
                    return;
                }
            }

            // --- ПРОВЕРКА 2: попали в ресурс (тег "Resource") ---
            if (hit.collider.CompareTag("Resource"))
            {
                // Получаем Entity ресурса
                Entity resourceEntity = hit.collider.GetComponent<Entity>();
                if (resourceEntity != null)
                {
                    // Отправляем КАЖДОГО выделенного союзника собирать этот ресурс
                    foreach (Entity unit in selectedUnits)
                    {
                        controller.GatherResource(unit, resourceEntity);
                    }
                    return;
                }
            }

            // --- ПРОВЕРКА 3: попали в землю (слой groundLayer) ---
            if (((1 << hit.collider.gameObject.layer) & groundLayer) != 0)
            {
                // Создаём временную точку в месте клика
                GameObject tempPoint = new GameObject("TempPoint");
                tempPoint.transform.position = hit.point;

                // Отправляем КАЖДОГО выделенного союзника в эту точку
                foreach (Entity unit in selectedUnits)
                {
                    controller.MoveToPosition(unit, tempPoint.transform);
                }

                // Удаляем временную точку
                Destroy(tempPoint);
            }
        }
    }
}