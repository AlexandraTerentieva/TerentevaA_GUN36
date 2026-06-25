using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class DOTweenPathMovement : MonoBehaviour
{
    [Header("Точки пути")]
    public List<Transform> waypoints; // список точек маршрута

    [Header("Настройки движения")]
    public float duration = 3f;          // время движения
    public Ease easeType = Ease.InOutQuad; // кривая скорости
    public float delay = 0f;             // задержка перед стартом

    [Header("Дополнительные эффекты")]
    public float scaleMultiplier = 1.5f; // увеличение размера
    public Color targetColor = Color.red; // цвет при движении

    private Vector3[] pathPositions;

    void Start()
    {
        // Преобразуем точки в массив позиций
        pathPositions = new Vector3[waypoints.Count];
        for (int i = 0; i < waypoints.Count; i++)
        {
            pathPositions[i] = waypoints[i].position;
        }

        // Запускаем движение
        StartMovement();
    }

    void StartMovement()
    {
        // 1. Движение по пути (один раз, без повторов)
        Tween moveTween = transform
            .DOPath(pathPositions, duration, PathType.CatmullRom)
            .SetEase(easeType)
            .SetDelay(delay)
            .OnComplete(() => Debug.Log("Персонаж дошёл до конца пути!"));

        // 2. Изменение масштаба (пульсация во время движения)
        Tween scaleTween = transform
            .DOScale(scaleMultiplier, duration / 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo);

        // 3. Изменение цвета
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Tween colorTween = renderer.material
                .DOColor(targetColor, duration / 2)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo);

            // Объединяем все твины
            Sequence sequence = DOTween.Sequence();
            sequence.Join(moveTween);
            sequence.Join(scaleTween);
            sequence.Join(colorTween);
            sequence.Play();
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Join(moveTween);
            sequence.Join(scaleTween);
            sequence.Play();
        }

        // 4. Эффект пыли (густые следы)
        InvokeRepeating("SpawnDust", 0.3f, 0.3f);
    }

    void SpawnDust()
    {
        // Создаём несколько частиц за раз
        for (int i = 0; i < 4; i++)
        {
            GameObject dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dust.transform.localScale = Vector3.one * Random.Range(0.15f, 0.4f);
            dust.transform.position = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), -0.3f, Random.Range(-0.3f, 0.3f));
            dust.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);

            // Исчезновение
            dust.transform
                .DOScale(Vector3.zero, 0.8f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(dust));
        }
    }
}