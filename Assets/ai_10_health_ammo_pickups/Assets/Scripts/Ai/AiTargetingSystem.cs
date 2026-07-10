using UnityEngine;

[ExecuteInEditMode]
public class AiTargetingSystem : MonoBehaviour
{
    public bool debug;
    public float memorySpan = 3.0f;
    public float distanceWeight = 1.0f;
    public float angleWeight = 1.0f;
    public float ageWeight = 1.0f;

    public bool HasTarget => bestMemory != null;

    public GameObject Target
    {
        get
        {
            if (bestMemory == null || bestMemory.gameObject == null) return null;
            return bestMemory.gameObject;
        }
    }

    public Vector3 TargetPosition
    {
        get
        {
            if (bestMemory == null || bestMemory.gameObject == null)
                return Vector3.zero;
            return bestMemory.gameObject.transform.position;
        }
    }

    public bool TargetInSight
    {
        get
        {
            if (bestMemory == null) return false;
            return bestMemory.Age < 0.5f;
        }
    }

    public float TargetDistance
    {
        get
        {
            if (bestMemory == null) return 0f;
            return bestMemory.distance;
        }
    }

    private AiSensoryMemory memory = new AiSensoryMemory();
    private AiSensor sensor;
    private AiMemory bestMemory;

    void Start()
    {
        sensor = GetComponent<AiSensor>();
    }

    void Update()
    {
        if (sensor == null) return;

        memory.UpdateSenses(sensor);
        memory.ForgetMemories(memorySpan);
        EvaluateScores();
    }

    void EvaluateScores()
    {
        bestMemory = null;

        foreach (var memoryItem in memory.memories)
        {
            if (memoryItem == null) continue;

            memoryItem.score = CalculateScore(memoryItem);

            if (bestMemory == null || memoryItem.score > bestMemory.score)
            {
                bestMemory = memoryItem;
            }
        }
    }

    float Normalize(float value, float maxValue)
    {
        if (maxValue <= 0f) return 1f;
        return Mathf.Clamp01(1f - (value / maxValue));
    }

    float CalculateScore(AiMemory memoryItem)
    {
        float distanceScore = Normalize(memoryItem.distance, sensor != null ? sensor.distance : 10f) * distanceWeight;
        float angleScore = Normalize(memoryItem.angle, sensor != null ? sensor.angle : 30f) * angleWeight;
        float ageScore = Normalize(memoryItem.Age, memorySpan) * ageWeight;
        return distanceScore + angleScore + ageScore;
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;

        float maxScore = float.MinValue;
        foreach (var memoryItem in memory.memories)
        {
            if (memoryItem != null)
                maxScore = Mathf.Max(maxScore, memoryItem.score);
        }

        foreach (var memoryItem in memory.memories)
        {
            if (memoryItem == null) continue;

            Color color = Color.red;
            if (memoryItem == bestMemory)
                color = Color.yellow;

            color.a = maxScore > 0 ? memoryItem.score / maxScore : 0.5f;
            Gizmos.color = color;
            Gizmos.DrawSphere(memoryItem.position, 0.2f);
        }
    }
}