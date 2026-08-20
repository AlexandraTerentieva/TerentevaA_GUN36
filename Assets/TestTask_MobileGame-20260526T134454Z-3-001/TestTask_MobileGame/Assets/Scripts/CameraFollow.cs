using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Камера с управлением мышью: вращается вокруг игрока.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Цель")]
    public Transform target;            // Игрок

    [Header("Смещение")]
    public float distance = 10f;        // Расстояние от игрока
    public float height = 5f;           // Высота над игроком

    [Header("Управление мышью")]
    public float mouseSensitivity = 3f; // Чувствительность мыши
    public float minVerticalAngle = 10f; // Минимальный угол вверх (градусы)
    public float maxVerticalAngle = 80f; // Максимальный угол вниз (градусы)

    [Header("Плавность")]
    public float smoothSpeed = 5f;      // Плавность движения

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private float currentX = 0f;        // Текущий угол по горизонтали
    private float currentY = 30f;       // Текущий угол по вертикали
    private Vector3 targetPosition;     // Целевая позиция камеры

    // ============================================================
    //  СТАРТ
    // ============================================================

    void Start()
    {
        // Блокируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Начальные углы (чтобы камера смотрела на игрока)
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    // ============================================================
    //  LATEUPDATE (после движения игрока)
    // ============================================================

    void LateUpdate()
    {
        if (target == null) return;

        // ============================================================
        //  УПРАВЛЕНИЕ МЫШЬЮ (только когда курсор скрыт!)
        // ============================================================

        // ✅ Если курсор виден для выделения — НЕ крутим камеру!
        if (UnitSelection.IsCursorLocked) return;

        // Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Изменяем углы
        currentX += mouseX;
        currentY -= mouseY;  // Инвертируем, чтобы вверх мыши = вверх камеры

        // Ограничиваем вертикальный угол (чтобы камера не улетала)
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // ============================================================
        //  ВЫЧИСЛЯЕМ ПОЗИЦИЮ КАМЕРЫ
        // ============================================================

        // Преобразуем углы в радианы
        float radiansX = currentX * Mathf.Deg2Rad;
        float radiansY = currentY * Mathf.Deg2Rad;

        // Вычисляем позицию камеры относительно игрока
        Vector3 offset = new Vector3(
            distance * Mathf.Sin(radiansX) * Mathf.Cos(radiansY),
            height + distance * Mathf.Sin(radiansY),
            distance * Mathf.Cos(radiansX) * Mathf.Cos(radiansY)
        );

        targetPosition = target.position + offset;

        // Плавное движение
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Камера всегда смотрит на игрока
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    // ============================================================
    //  ОТЛАДКА (переключение курсора по Escape)
    // ============================================================

    void Update()
    {
        // ✅ Если курсор виден для выделения — НЕ ТРОГАЕМ ЕГО!
        if (UnitSelection.IsCursorLocked) return;

        // На Escape — показываем курсор
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Клик по экрану — скрываем курсор
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}