using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Камера с управлением мышью: вращается вокруг игрока.
/// Если курсор виден (режим выделения) — камера не вращается.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // ============================================================
    //  НАСТРОЙКИ В ИНСПЕКТОРЕ
    // ============================================================

    [Header("Цель")]
    public Transform target;            // Игрок

    [Header("Смещение")]
    public float distance = 15f;
    public float height = 8f;

    [Header("Управление мышью")]
    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    [Header("Плавность")]
    public float smoothSpeed = 5f;

    // ============================================================
    //  ПРИВАТНЫЕ ПЕРЕМЕННЫЕ
    // ============================================================

    private float currentX = 0f;
    private float currentY = 45f;
    private Vector3 targetPosition;

    // ============================================================
    //  СТАРТ
    // ============================================================

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = 45f;
    }

    // ============================================================
    //  LATEUPDATE
    // ============================================================

    void LateUpdate()
    {
        if (target == null) return;

        // ============================================================
        //  ✅ ЕСЛИ КУРСОР ВИДЕН — НЕ ВРАЩАЕМ КАМЕРУ!
        // ============================================================
        if (UnitSelection.IsCursorVisible)
        {
            // Камера не вращается, но продолжает следовать за игроком
            UpdateCameraPosition();
            return;
        }

        // Управление мышью (только когда курсор скрыт)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentX += mouseX;
        currentY += mouseY;

        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        UpdateCameraPosition();
    }

    // ============================================================
    //  МЕТОД: ОБНОВЛЕНИЕ ПОЗИЦИИ КАМЕРЫ
    // ============================================================

    private void UpdateCameraPosition()
    {
        float radiansX = currentX * Mathf.Deg2Rad;
        float radiansY = currentY * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            distance * Mathf.Sin(radiansX) * Mathf.Cos(radiansY),
            height + distance * Mathf.Sin(radiansY),
            distance * Mathf.Cos(radiansX) * Mathf.Cos(radiansY)
        );

        targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    // ============================================================
    //  UPDATE (курсор)
    // ============================================================

    void Update()
    {
        // Если курсор виден — не трогаем его через Escape
        if (UnitSelection.IsCursorVisible) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UnitSelection.IsCursorVisible = true; // ← синхронизируем
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            UnitSelection.IsCursorVisible = false; // ← синхронизируем
        }
    }
}