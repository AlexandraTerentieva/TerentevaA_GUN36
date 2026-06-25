using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterStateManager : MonoBehaviour
{
    public enum State { Idle, Search, Collect, Return }
    public State currentState = State.Idle;

    public float idleDuration = 3f;
    public float collectRadius = 5f;
    public int maxCollectibles = 5;
    public Transform returnPoint;

    public Text scoreText;

    private NavMeshAgent agent;
    private Animator animator;
    private GameObject targetCollectible;
    private int collectedCount = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        UpdateScoreUI();
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Idle:
                    yield return StartCoroutine(Idle());
                    break;
                case State.Search:
                    yield return StartCoroutine(Search());
                    break;
                case State.Collect:
                    yield return StartCoroutine(Collect());
                    break;
                case State.Return:
                    yield return StartCoroutine(ReturnToStart());
                    break;
            }
        }
    }

    IEnumerator Idle()
    {
        animator.SetTrigger("Idle");
        agent.ResetPath();
        yield return new WaitForSeconds(idleDuration);
        currentState = State.Search;
    }

    IEnumerator Search()
    {
        animator.SetTrigger("Search");
        targetCollectible = GetNearestCollectible();

        if (targetCollectible != null)
        {
            agent.SetDestination(targetCollectible.transform.position);
            yield return new WaitForSeconds(0.5f);
            currentState = State.Collect;
        }
        else
        {
            SetRandomDestination();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator Collect()
    {
        if (targetCollectible == null || !targetCollectible.activeSelf)
        {
            currentState = State.Search;
            yield break;
        }

        animator.SetTrigger("Collect");
        agent.SetDestination(targetCollectible.transform.position);

        float distance = Vector3.Distance(transform.position, targetCollectible.transform.position);
        if (distance <= 1.5f)
        {
            targetCollectible.SetActive(false);
            collectedCount++;
            UpdateScoreUI();
            targetCollectible = null;

            if (collectedCount >= maxCollectibles)
            {
                currentState = State.Return;
            }
            else
            {
                currentState = State.Search;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            currentState = State.Collect;
        }
    }

    IEnumerator ReturnToStart()
    {
        animator.SetTrigger("Return");
        agent.SetDestination(returnPoint.position);

        while (Vector3.Distance(transform.position, returnPoint.position) > 1.5f)
        {
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Персонаж вернулся на базу!");
        collectedCount = 0;
        UpdateScoreUI();
        currentState = State.Idle;
    }

    void SetRandomDestination()
    {
        Vector3 randomPos = Random.insideUnitSphere * 10f + transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    GameObject GetNearestCollectible()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Collectible");
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject item in items)
        {
            if (item == null || !item.activeSelf) continue;
            float dist = Vector3.Distance(transform.position, item.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = item;
            }
        }
        return nearest;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Собрано: " + collectedCount + "/" + maxCollectibles;
        }
    }
}