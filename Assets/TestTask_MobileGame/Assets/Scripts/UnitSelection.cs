using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Выделение врагов по клику мышкой.
/// Нажмите Tab, чтобы показать/скрыть курсор.
/// Выделенный враг увеличивается в размере.
/// </summary>
public class UnitSelection : MonoBehaviour
{
    // ============================================================
    //  ПУБЛИЧНЫЙ ФЛАГ ДЛЯ КАМЕРЫ
    //  Используется в CameraFollow, чтобы отключать вращение камеры
    // ============================================================

    public static bool IsCursorVisible = false;

    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Размер")]
    public float normalScale = 1f;        // Обычный размер врага
    public float selectedScale = 1.3f;    // Размер выделенного врага

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private List<GameObject> selectedEnemies = new List<GameObject>(); // Список выделенных врагов
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>(); // Исходные размеры врагов

    // ============================================================
    //  СТАРТ
    //  Вызывается при запуске игры
    // ============================================================

    void Start()
    {
        // При старте курсор скрыт (игровой режим)
        IsCursorVisible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ============================================================
    //  ОБНОВЛЕНИЕ
    //  Вызывается каждый кадр
    // ============================================================

    void Update()
    {
        // ============================================================
        //  ПЕРЕКЛЮЧЕНИЕ КУРСОРА ПО TAB
        //  Нажали Tab — показываем курсор, ещё раз — скрываем
        // ============================================================

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            IsCursorVisible = !IsCursorVisible;

            if (IsCursorVisible)
            {
                // Показываем курсор (режим выделения)
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("🖱️ Курсор ПОКАЗАН (кликай по врагам для выделения)");
            }
            else
            {
                // Скрываем курсор (игровой режим)
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                ClearSelection();
                Debug.Log("🖱️ Курсор СКРЫТ");
            }
        }

        // ============================================================
        //  ВЫДЕЛЕНИЕ ПО КЛИКУ (Левая кнопка мыши)
        //  Работает ТОЛЬКО когда курсор виден
        // ============================================================

        if (IsCursorVisible && Input.GetMouseButtonDown(0))
        {
            // Пускаем луч из камеры в точку клика
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем, попали ли во что-то
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Проверяем, есть ли у объекта тег "Enemy"
                if (hitObject.CompareTag("Enemy"))
                {
                    // Если враг уже выделен — снимаем выделение
                    if (selectedEnemies.Contains(hitObject))
                    {
                        SetEnemyScale(hitObject, normalScale);
                        selectedEnemies.Remove(hitObject);
                        Debug.Log($"❌ Снято выделение с {hitObject.name}");
                    }
                    else
                    {
                        // Сохраняем исходный размер, если ещё не сохранили
                        if (!originalScales.ContainsKey(hitObject))
                        {
                            originalScales[hitObject] = hitObject.transform.localScale;
                        }
                        selectedEnemies.Add(hitObject);
                        SetEnemyScale(hitObject, selectedScale);
                        Debug.Log($"✅ Выделен враг: {hitObject.name}");
                    }
                }
                else
                {
                    // Кликнули не по врагу — снимаем всё выделение
                    ClearSelection();
                }
            }
            else
            {
                // Кликнули в пустоту — снимаем всё выделение
                ClearSelection();
            }
        }

        // ============================================================
        //  СНЯТИЕ ВЫДЕЛЕНИЯ ПО ESC
        // ============================================================

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
            if (IsCursorVisible)
            {
                IsCursorVisible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("🖱️ Курсор СКРЫТ (ESC)");
            }
        }
    }

    // ============================================================
    //  МЕТОД: СНЯТИЕ ВЫДЕЛЕНИЯ
    // ============================================================

    private void ClearSelection()
    {
        // Возвращаем размер всем выделенным врагам
        foreach (var enemy in selectedEnemies)
        {
            if (enemy != null)
            {
                SetEnemyScale(enemy, normalScale);
            }
        }
        selectedEnemies.Clear();
        originalScales.Clear();
        Debug.Log("❌ Выделение снято");
    }

    // ============================================================
    //  МЕТОД: ИЗМЕНЕНИЕ РАЗМЕРА ВРАГА
    // ============================================================

    private void SetEnemyScale(GameObject enemy, float scaleMultiplier)
    {
        if (enemy != null)
        {
            if (originalScales.ContainsKey(enemy))
            {
                // Используем сохранённый исходный размер
                enemy.transform.localScale = originalScales[enemy] * scaleMultiplier;
            }
            else
            {
                // Если исходный размер не сохранён — используем текущий
                enemy.transform.localScale = Vector3.one * scaleMultiplier;
            }
        }
    }
}