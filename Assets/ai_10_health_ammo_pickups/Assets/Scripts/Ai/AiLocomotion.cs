using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    AudioSource footstepAudio;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        footstepAudio = GetComponent<AudioSource>();

        if (footstepAudio != null)
        {
            Debug.Log("[AiLocomotion] AudioSource найден");
        }
        else
        {
            Debug.LogWarning("[AiLocomotion] AudioSource НЕ найден!");
        }
    }

    void Update()
    {
        if (agent.hasPath)
        {
            animator.SetFloat("speed", agent.velocity.magnitude);
        }
        else
        {
            animator.SetFloat("speed", 0);
        }

        if (footstepAudio != null)
        {
            float speed = agent.velocity.magnitude;
            bool isMoving = speed > 0.1f;

            Debug.Log("[Шаги] Скорость: " + speed + " | Двигается: " + isMoving + " | AudioSource играет: " + footstepAudio.isPlaying);

            if (isMoving && !footstepAudio.isPlaying)
            {
                footstepAudio.Play();
                Debug.Log("[Шаги] Включил звук!");
            }
            else if (!isMoving && footstepAudio.isPlaying)
            {
                footstepAudio.Stop();
                Debug.Log("[Шаги] Выключил звук");
            }
        }
    }
}