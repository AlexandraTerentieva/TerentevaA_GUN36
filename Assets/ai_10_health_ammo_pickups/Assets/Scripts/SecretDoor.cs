using UnityEngine;

public class SecretDoor : MonoBehaviour
{
    public float openAngle = -90f; // Поворот от края
    public float speed = 2f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        // Поворачиваем вокруг локальной оси Y
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = true;
        }
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }
}