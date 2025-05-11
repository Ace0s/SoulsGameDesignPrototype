using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [Header("References")] 
    public PlayerCamera playerCamera; // Assign this in Inspector
    private PlayerCombat combat;
    
    [Header("Movement Variables")] 
    public float speed = 5f;
   
    [Header("Dodge Roll")]
    public float rollSpeed = 8f;
    public float rollDuration = 0.5f;
    private bool isRolling;
    private Vector3 rollDirection;
    private float rollTimer;
    
    public float rollCooldown = 0.2f;
    private bool canRoll = true;
    public bool isInvincible = false;
    private float cooldownTimer = 0f;

    private void Start()
    {
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if (combat != null && combat.IsAttacking()) return;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 moveDir;

            if (playerCamera.IsLockedOn() && playerCamera.GetCurrentTarget() != null)
            {
                // Get direction from player to enemy
                Vector3 toTarget = (playerCamera.GetCurrentTarget().position - transform.position).normalized;
                toTarget.y = 0f;

                // Use the direction to the enemy as forward reference
                Quaternion lockOnRotation = Quaternion.LookRotation(toTarget);
                moveDir = lockOnRotation * inputDir;

                // Face the enemy (permanently while locked)
                transform.rotation = Quaternion.Slerp(transform.rotation, lockOnRotation, Time.deltaTime * 10f);
            }
            else
            {
                // Standard free movement
                Quaternion camYaw = playerCamera.GetCameraYawRotation();
                moveDir = camYaw * inputDir;

                // Rotate toward movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // Apply movement
            transform.position += moveDir * (speed * Time.deltaTime);
        }
        
        if (isRolling)
        {
            transform.position += rollDirection * (rollSpeed * Time.deltaTime);
            rollTimer -= Time.deltaTime;

            if (rollTimer <= 0f)
            {
                isRolling = false;
                isInvincible = false;
            }

            return; // Skip regular movement while rolling
        }
        
        if (!canRoll)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canRoll = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && canRoll)
        {
            isInvincible = true;
            canRoll = false;
            cooldownTimer = rollCooldown;
            
            Vector3 rollInputDir = new Vector3(horizontal, 0f, vertical).normalized;

            if (playerCamera.IsLockedOn() && playerCamera.GetCurrentTarget() != null)
            {
                // Movement relative to lock-on forward
                Vector3 toTarget = (playerCamera.GetCurrentTarget().position - transform.position).normalized;
                Quaternion referenceRot = Quaternion.LookRotation(new Vector3(toTarget.x, 0, toTarget.z));
                rollDirection = referenceRot * rollInputDir;
            }
            else
            {
                // Movement relative to camera
                Quaternion camYaw = playerCamera.GetCameraYawRotation();
                rollDirection = camYaw * rollInputDir;
            }

            // If no input, roll forward
            if (rollDirection.magnitude < 0.1f)
                rollDirection = transform.forward;

            isRolling = true;
            rollTimer = rollDuration;

            // Optional: trigger roll animation
            // animator.SetTrigger("Roll");
        }
    }
}