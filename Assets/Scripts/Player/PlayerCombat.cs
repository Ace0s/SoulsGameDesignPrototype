using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    
    [Header("Health Variables")] 
    public float currentHealth = 100;
    public float health = 100;
    public float maxHealth = 100;
    
     [Header("Attack Variables")]
    public float attackDuration = 0.6f;
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;

    private bool isAttacking = false;
    private float attackTimer = 0f;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartAttack();
        }

        if (health < currentHealth)
        {
            currentHealth = health;
            Debug.Log("PlayerHurt");
        } else if (health <= 0)
        {
            Debug.Log("Player Died");
            Destroy(gameObject);
        }
        
        
    }
    
    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        // Trigger animation
        if (animator != null)
            animator.SetTrigger("Attack");
    }
    
    void DoAttack()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hits)
        {
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(10);
        }
    }

    public bool IsAttacking() => isAttacking;

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
