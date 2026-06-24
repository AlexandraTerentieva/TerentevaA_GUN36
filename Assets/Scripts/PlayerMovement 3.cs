using UnityEngine;

public class PlayerMovementIsometric : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(x, y) * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }
}