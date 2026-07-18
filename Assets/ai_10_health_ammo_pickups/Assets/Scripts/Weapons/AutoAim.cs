using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    public float range = 50f;
    public LayerMask targetLayer;
    public Transform weaponPivot;
    public float rotationSpeed = 5f;

    private Transform currentTarget;
    private RaycastWeapon weapon;
    private bool isPlayer = false; 

    void Start()
    {
        weapon = GetComponent<RaycastWeapon>();
        if (weaponPivot == null)
            weaponPivot = transform;

        if (transform.root.CompareTag("Player"))
        {
            isPlayer = true;
            enabled = false;
            Debug.Log("AutoAim отключен для игрока");
        }
        else
        {
            Debug.Log("AutoAim активен для врага");
        }
    }

    void Update()
    {
        if (isPlayer) return; 

        currentTarget = FindClosestTarget();

        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - weaponPivot.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            weaponPivot.rotation = Quaternion.Slerp(weaponPivot.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (weapon != null && Vector3.Distance(transform.position, currentTarget.position) <= range)
            {
                weapon.StartFiring();
            }
        }
        else
        {
            if (weapon != null)
                weapon.StopFiring();
        }
    }

    Transform FindClosestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDistance = range;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = target.transform;
            }
        }

        return closest;
    }
}