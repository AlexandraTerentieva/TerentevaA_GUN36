using UnityEngine;
using UnityEngine.UI; // если используешь UI для очков

public class BowlingController : MonoBehaviour
{
    [Header("Мяч")]
    public GameObject ballPrefab;      // префаб мяча
    public Transform spawnPoint;       // точка броска
    public float throwForce = 20f;     // сила броска

    [Header("Кегли")]
    public GameObject[] pins;          // все кегли (с тегом Pin)

    [Header("UI")]
    public Text scoreText;             // текст для очков

    private GameObject currentBall;
    private int currentScore = 0;
    private int throwCount = 0;        // 1 или 2 бросок в фрейме
    private bool hasThrown = false;

    void Start()
    {
        // Находим все кегли по тегу
        pins = GameObject.FindGameObjectsWithTag("Pin");
        SpawnBall();
        UpdateScoreUI();
    }

    void Update()
    {
        // Бросок по клику мыши
        if (Input.GetMouseButtonDown(0) && !hasThrown && currentBall != null)
        {
            Rigidbody rb = currentBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                // Бросок вперёд от камеры
                Vector3 direction = Camera.main.transform.forward;
                direction.y = 0;
                rb.AddForce(direction * throwForce, ForceMode.Impulse);
                hasThrown = true;
                throwCount++;
                Invoke(nameof(CheckPins), 3f);
            }
        }

        // Сброс фрейма (новый подход) — по пробелу
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetFrame();
        }
    }

    void SpawnBall()
    {
        if (currentBall != null) Destroy(currentBall);
        currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        hasThrown = false;
    }

    void CheckPins()
    {
        int fallen = CountFallenPins();
        int points = CalculatePoints(fallen);
        currentScore += points;
        UpdateScoreUI();

        Debug.Log($"Сбито: {fallen}, очков за бросок: {points}, всего: {currentScore}");

        // Автоматический сброс через 2 секунды
        Invoke(nameof(ResetFrame), 2f);
    }

    int CountFallenPins()
    {
        int fallen = 0;
        foreach (GameObject pin in pins)
        {
            if (pin == null) continue;
            // Кегля упала, если её верх направлен вниз
            if (pin.transform.up.y < 0.5f)
                fallen++;
        }
        return fallen;
    }

    int CalculatePoints(int fallen)
    {
        if (fallen == 10 && throwCount == 1)
            return 30; // Страйк: 10 + 20 бонусных
        else if (fallen == 10 && throwCount == 2)
            return 20; // Спар: 10 + 10 бонусных
        else
            return fallen; // 1 очко за каждую сбитую
    }

    void ResetFrame()
    {
        throwCount = 0;
        SpawnBall();
        ResetPins();
    }

    void ResetPins()
    {
        // Здесь нужно пересоздать кегли или вернуть их в исходное положение
        // Можно просто перезагрузить сцену или использовать готовый префаб расстановки
        Debug.Log("Кегли сброшены (нужно реализовать возврат позиций)");
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Очки: " + currentScore;
    }
}