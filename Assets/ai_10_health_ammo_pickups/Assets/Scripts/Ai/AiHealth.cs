using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHealth : Health
{
    AiAgent agent;
    public GameObject explosionPrefab;
    public GameObject bloodPrefab;
    public AudioClip explosionSound;

    private bool isDead = false; // 👈 БЛОКИРОВКА ПОВТОРНЫХ ВЫЗОВОВ

    protected override void OnStart()
    {
        agent = GetComponent<AiAgent>();
    }

    protected override void OnDeath(Vector3 direction)
    {
        if (isDead) return; // 👈 ЕСЛИ УЖЕ МЁРТВ — НЕ ПОВТОРЯЕМ
        isDead = true;

        // Взрыв (только один раз)
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Звук взрыва (только один раз)
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Переключаем состояние на Death
        AiDeathState deathState = agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
        if (deathState != null)
        {
            deathState.direction = direction;
        }
        agent.stateMachine.ChangeState(AiStateId.Death);

        // Уничтожаем врага через 2 секунды
        Destroy(gameObject, 2f);
    }

    protected override void OnDamage(Vector3 direction)
    {
        if (isDead) return; // 👈 НЕ СОЗДАЁМ КРОВЬ, ЕСЛИ МЁРТВ

        if (bloodPrefab != null)
        {
            Instantiate(bloodPrefab, transform.position, Quaternion.identity);
        }
    }
}