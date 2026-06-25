using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BowlingController : MonoBehaviour
{
    [Header("ћ€ч")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float throwForce = 20f;

    [Header(" егли")]
    [SerializeField] private Transform[] pinPositions;

    [Header("UI")]
    [SerializeField] private Text scoreText;

    private GameObject currentBall;
    private List<GameObject> pins = new List<GameObject>();
    private List<Vector3> initialPinPositions = new List<Vector3>();
    private List<Quaternion> initialPinRotations = new List<Quaternion>();

    private int currentScore = 0;
    private int throwCount = 0;
    private bool hasThrown = false;

    void Start()
    {
        // Ќаходим все кегли по тегу Pin
        GameObject[] foundPins = GameObject.FindGameObjectsWithTag("Pin");
        pins = new List<GameObject>(foundPins);

        // —охран€ем начальные позиции и повороты
        foreach (GameObject pin in pins)
        {
            initialPinPositions.Add(pin.transform.position);
            initialPinRotations.Add(pin.transform.rotation);
        }

        SpawnBall();
        UpdateScoreUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasThrown && currentBall != null)
        {
            Rigidbody rb = currentBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                Vector3 direction = Camera.main.transform.forward;
                direction.y = 0;
                rb.AddForce(direction * throwForce, ForceMode.Impulse);
                hasThrown = true;
                throwCount++;
                StartCoroutine(CheckPinsAfterDelay(3f));
            }
        }

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
        }
        hasThrown = false;
    }

    IEnumerator CheckPinsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CountFallenPins();
    }

    void CountFallenPins()
    {
        int fallen = 0;
        foreach (GameObject pin in pins)
        {
            if (pin == null) continue;
            if (Vector3.Angle(pin.transform.up, Vector3.up) > 45f)
                fallen++;
        }

        int points = CalculatePoints(fallen);
        currentScore += points;
        UpdateScoreUI();

        Debug.Log($"—бито: {fallen} из {pins.Count}, очков за бросок: {points}, всего: {currentScore}");

        Invoke(nameof(ResetFrame), 2f);
    }

    int CalculatePoints(int fallen)
    {
        if (fallen == 10 && throwCount == 1)
            return 30;
        else if (fallen == 10 && throwCount == 2)
            return 20;
        else
            return fallen;
    }

    void ResetFrame()
    {
        throwCount = 0;
        ResetPins();
        SpawnBall();
    }

    void ResetPins()
    {
        for (int i = 0; i < pins.Count; i++)
        {
            if (pins[i] == null) continue;

            pins[i].transform.position = initialPinPositions[i];
            pins[i].transform.rotation = initialPinRotations[i];

            Rigidbody rb = pins[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                bool wasKinematic = rb.isKinematic;
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = wasKinematic;
            }
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "ќчки: " + currentScore;
    }
}