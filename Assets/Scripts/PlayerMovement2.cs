using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float speed = 5f;         // скорость движения
    public float jumpForce = 7f;     // сила прыжка
    public Transform groundCheck;    // точка проверки земли
    public float checkRadius = 0.3f; // радиус проверки
    public LayerMask groundLayer;    // слой земли

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Проверяем, стоит ли персонаж на земле
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // Движение
        float x = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(x * speed, rb.velocity.y);

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}