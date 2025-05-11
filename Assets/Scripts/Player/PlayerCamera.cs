using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public float sensitivity = 3f;
    public float distance = 6f;
    public float height = 3f;
    public LayerMask enemyLayer;
    public float lockOnRange = 15f;
    public Transform currentTarget;

    private float yaw = 0f;
    private float pitch = 0f;
    private bool isLockedOn = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isLockedOn)
                ClearLockOn();
            else
                TryLockOn();
        }

        if (isLockedOn && currentTarget != null)
        {
            // Stay behind the player relative to the direction between player and target
            Vector3 directionToTarget = (currentTarget.position - player.position).normalized;
            Vector3 flatDirection = new Vector3(directionToTarget.x, 0f, directionToTarget.z).normalized;

            // Place camera behind the player based on direction to target
            Vector3 lockCamOffset = -flatDirection * distance + Vector3.up * height;
            transform.position = Vector3.Lerp(transform.position, player.position + lockCamOffset, Time.deltaTime * 10f);

            // Look at the enemy
            transform.LookAt(currentTarget.position + Vector3.up * 2.5f);
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch, -20f, 60f);

            // Compute camera rotation from mouse input
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            // Desired position behind the player based on mouse
            Vector3 offsetPos = player.position - rotation * Vector3.forward * distance + Vector3.up * height;
            transform.position = Vector3.Lerp(transform.position, offsetPos, Time.deltaTime * 10f);

            // Always look at the player, but rotation is controlled by the orbit
            transform.LookAt(player.position + Vector3.up * 1.5f); // Slight upward offset for better framing
        }
    }

    public Quaternion GetCameraYawRotation()
    {
        return Quaternion.Euler(0, yaw, 0);
    }

    void TryLockOn()
    {
        Collider[] enemies = Physics.OverlapSphere(player.position, lockOnRange, enemyLayer);
        Transform bestTarget = null;
        float closestAngle = 60f; // Only consider enemies in front

        foreach (var enemy in enemies)
        {
            Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);
            if (angle < closestAngle)
            {
                closestAngle = angle;
                bestTarget = enemy.transform;
            }
        }

        if (bestTarget != null)
        {
            currentTarget = bestTarget;
            isLockedOn = true;
            
            // Enable marker
            Transform marker = currentTarget.Find("TargetMarker");
            if (marker != null) marker.gameObject.SetActive(true);
        }
    }

    void ClearLockOn()
    {
        if (currentTarget != null)
        {
            Transform marker = currentTarget.Find("TargetMarker");
            if (marker != null) marker.gameObject.SetActive(false);
        }
        
        isLockedOn = false;
        currentTarget = null;

        // Sync yaw and pitch to current camera rotation so there's no snap
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    public bool IsLockedOn() => isLockedOn;
    public Transform GetCurrentTarget() => currentTarget;
}