using System.Collections.Generic;
using UnityEngine;

public class AiMemory
{
    public float Age => Time.time - lastSeen;
    public GameObject gameObject;
    public Vector3 position;
    public Vector3 direction;
    public float distance;
    public float angle;
    public float lastSeen;
    public float score;
}

public class AiSensoryMemory
{
    public List<AiMemory> memories = new List<AiMemory>();
    private GameObject[] characters;

    public AiSensoryMemory()
    {
        characters = new GameObject[10];
    }

    public void UpdateSenses(AiSensor sensor)
    {
        if (sensor == null) return;

        int targets = sensor.Filter(characters, "Character", null);
        for (int i = 0; i < targets; i++)
        {
            GameObject target = characters[i];
            if (target != null && target.CompareTag("Player"))
            {
                RefreshMemory(sensor.gameObject, target);
            }
        }
    }

    public void RefreshMemory(GameObject agent, GameObject target)
    {
        if (agent == null || target == null) return;

        AiMemory memory = FetchMemory(target);
        memory.gameObject = target;
        memory.position = target.transform.position;
        memory.direction = target.transform.position - agent.transform.position;
        memory.distance = memory.direction.magnitude;
        memory.angle = Vector3.Angle(agent.transform.forward, memory.direction);
        memory.lastSeen = Time.time;
    }

    public AiMemory FetchMemory(GameObject gameObject)
    {
        AiMemory memory = memories.Find(x => x.gameObject == gameObject);
        if (memory == null)
        {
            memory = new AiMemory();
            memories.Add(memory);
        }
        return memory;
    }

    public void ForgetMemories(float olderThan)
    {
        memories.RemoveAll(m =>
            m.Age > olderThan ||
            m.gameObject == null ||
            m.gameObject.GetComponent<Health>() == null ||
            m.gameObject.GetComponent<Health>().IsDead()
        );
    }
}