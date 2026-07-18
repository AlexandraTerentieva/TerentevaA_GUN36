using UnityEngine;

public class DestroyOnDamage : MonoBehaviour
{
    public int health = 1;
    public GameObject explosionPrefab; 

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            Debug.Log("Объект разрушен!");
        }
    }
}