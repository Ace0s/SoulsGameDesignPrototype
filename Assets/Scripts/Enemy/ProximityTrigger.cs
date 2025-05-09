using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyHealth?.TriggerBossIntro();
        }
    }
}
