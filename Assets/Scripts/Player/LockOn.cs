using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
     public Transform cameraTransform; // Reference to the camera
    public float lockOnRadius = 10f;  // Maximum distance for lock-on
    public KeyCode lockOnKey = KeyCode.Q; // Key to trigger lock-on

    private Transform currentTarget;

    private void Update()
    {
        if (Input.GetKeyDown(lockOnKey))
        {
            // Find the nearest target within the lock-on radius
            currentTarget = FindNearestTarget();

            if (currentTarget != null)
            {
                // Point the camera at the target
                Vector3 directionToTarget = currentTarget.position - cameraTransform.position;
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        else if (Input.GetKeyUp(lockOnKey))
        {
            // Unlock the camera
            currentTarget = null;
        }
    }

    private Transform FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, lockOnRadius);
        float closestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy")) // Ensure the target is an enemy
            {
                Vector3 directionToTarget = collider.transform.position - transform.position;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget < 90f) // Check if the target is within the field of view
                {
                    float distanceToTarget = directionToTarget.magnitude;

                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        nearestTarget = collider.transform;
                    }
                }
            }
        }

        return nearestTarget;
    }
}
