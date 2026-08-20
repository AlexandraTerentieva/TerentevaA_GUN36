using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Выделение врагов по клику мышкой.
/// Нажмите Tab, чтобы показать/скрыть курсор.
/// </summary>
public class UnitSelection : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Размер")]
    public float normalScale = 1f;
    public float selectedScale = 1.3f;

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private List<GameObject> selectedEnemies = new List<GameObject>();
    private bool isCursorVisible = false;
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    // СТАТИЧЕСКИЙ ФЛАГ — ДОСТУПЕН ИЗ ДРУГИХ СКРИПТОВ!
    public static bool IsCursorLocked = false;

    // ============================================================
    //  СТАРТ
    // ============================================================

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorVisible = false;
        IsCursorLocked = false;
    }

    // ============================================================
    //  ОБНОВЛЕНИЕ
    // ============================================================

    void Update()
    {
        // ============================================================
        //  ПЕРЕКЛЮЧЕНИЕ КУРСОРА ПО TAB
        // ============================================================

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isCursorVisible = !isCursorVisible;

            if (isCursorVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                IsCursorLocked = true;  // ← ЗАПРЕЩАЕМ ДРУГИМ СКРИПТАМ МЕНЯТЬ КУРСОР
                Debug.Log("🖱️ Курсор ПОКАЗАН (кликай по врагам)");
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                IsCursorLocked = false; // ← РАЗРЕШАЕМ ДРУГИМ СКРИПТАМ УПРАВЛЯТЬ КУРСОРОМ
                ClearSelection();
                Debug.Log("🖱️ Курсор СКРЫТ");
            }
        }

        // ============================================================
        //  ВЫДЕЛЕНИЕ (только когда курсор виден)
        // ============================================================

        if (isCursorVisible && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Enemy"))
                {
                    if (selectedEnemies.Contains(hitObject))
                    {
                        SetEnemyScale(hitObject, normalScale);
                        selectedEnemies.Remove(hitObject);
                        Debug.Log($"❌ Снято выделение с {hitObject.name}");
                    }
                    else
                    {
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
                    ClearSelection();
                }
            }
            else
            {
                ClearSelection();
            }
        }

        // ============================================================
        //  ОТПРАВКА В ТОЧКУ (ПКМ)
        // ============================================================

        if (isCursorVisible && Input.GetMouseButtonDown(1) && selectedEnemies.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 targetPoint = hit.point;
                Debug.Log($"🎯 Отправляем {selectedEnemies.Count} врагов в точку {targetPoint}");

                foreach (var enemy in selectedEnemies)
                {
                    if (enemy != null)
                    {
                        enemy.transform.position = Vector3.MoveTowards(
                            enemy.transform.position,
                            targetPoint,
                            5f * Time.deltaTime
                        );
                    }
                }
            }
        }

        // ============================================================
        //  СНЯТИЕ ВЫДЕЛЕНИЯ ПО ESC
        // ============================================================

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
            if (isCursorVisible)
            {
                isCursorVisible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                IsCursorLocked = false;
                Debug.Log("🖱️ Курсор СКРЫТ (ESC)");
            }
        }
    }

    // ============================================================
    //  МЕТОДЫ
    // ============================================================

    private void ClearSelection()
    {
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

    private void SetEnemyScale(GameObject enemy, float scaleMultiplier)
    {
        if (enemy != null)
        {
            if (originalScales.ContainsKey(enemy))
            {
                enemy.transform.localScale = originalScales[enemy] * scaleMultiplier;
            }
            else
            {
                enemy.transform.localScale = Vector3.one * scaleMultiplier;
            }
        }
    }
}