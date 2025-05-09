using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    
    
    public string bossName = "Gravel Guard";
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent<string> onBossEngaged;
    public UnityEvent onBossDefeated;

    private bool isEngaged = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (!isEngaged)
        {
            isEngaged = true;
            onBossEngaged?.Invoke(bossName);
        }

        currentHealth -= amount;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            onBossDefeated?.Invoke();
            Destroy(gameObject);
        }
    }
    
    public void TriggerBossIntro()
    {
        if (!isEngaged)
        {
            isEngaged = true;
            onBossEngaged?.Invoke(bossName);
            onHealthChanged?.Invoke(currentHealth, maxHealth); // Show full bar
        }
    }
}
