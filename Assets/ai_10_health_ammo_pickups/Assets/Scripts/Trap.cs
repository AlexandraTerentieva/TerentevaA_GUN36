using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 10;
    public GameObject effectPrefab; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, Vector3.zero);
                Debug.Log("Игрок попал в ловушку!");

                if (effectPrefab != null)
                {
                    Instantiate(effectPrefab, transform.position, Quaternion.identity);
                }
            }
        }
    }
}