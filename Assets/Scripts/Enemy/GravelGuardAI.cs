using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravelGuardAI : MonoBehaviour
{
    public float walkSpeed = 1.5f;
        public float attackRange = 3f;
        public float timeBetweenAttacks = 3f;
    
        public Transform player;
        private EnemyHealth bossHealth;
    
        private bool isAttacking = false;
        private float lastAttackTime;
        
        private float groundedY;
    
        void Start()
        {
            bossHealth = GetComponent<EnemyHealth>();
            lastAttackTime = -timeBetweenAttacks;
            groundedY = transform.position.y;
        }
    
        void Update()
        {
            // Lock Y to original grounded position
            Vector3 pos = transform.position;
            pos.y = groundedY;
            transform.position = pos;
            
            if (player == null || bossHealth == null) return;
    
            float distance = Vector3.Distance(transform.position, player.position);
    
            if (!isAttacking)
            {
                if (distance > attackRange)
                {
                    // Move toward player slowly
                    Vector3 direction = (player.position - transform.position).normalized;
                    transform.position += direction * (walkSpeed * Time.deltaTime);
                    direction.y = 0f; // Ignore vertical difference
                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                    }
                }
                else if (Time.time - lastAttackTime >= timeBetweenAttacks)
                {
                    StartCoroutine(PerformAttack());
                }
            }
        }
    
        IEnumerator PerformAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;
    
            int attackType = ChooseAttack(); // Weighted
    
            switch (attackType)
            {
                case 1:
                    yield return Attack1_Swing();
                    break;
                case 2:
                    yield return Attack2_MediumHit();
                    break;
                case 3:
                    yield return Attack3_Slam();
                    break;
            }
    
            isAttacking = false;
        }
    
        int ChooseAttack()
        {
            int roll = Random.Range(0, 100);
            if (roll < 60) return 1;      // 60% chance
            if (roll < 90) return 2;      // 30% chance
            return 3;                     // 10% chance
        }
    
        IEnumerator Attack1_Swing()
        {
            Debug.Log("Boss: Swing Attack!");
            ShowHitbox(transform.position + transform.forward * 2f, 1f, 0.4f); // small hitbox
            yield return new WaitForSeconds(0.8f);
            DealDamageInArea(transform.position + transform.forward * 2f, 1f, 20f);
        }
    
        IEnumerator Attack2_MediumHit()
        {
            Debug.Log("Boss: Medium Attack!");
            ShowHitbox(transform.position + transform.forward * 2.5f, 2f, 0.7f);
            yield return new WaitForSeconds(1.2f);
            DealDamageInArea(transform.position + transform.forward * 2.5f, 2f, 30f);
        }
    
        IEnumerator Attack3_Slam()
        {
            Debug.Log("Boss: Slam!");
            ShowHitbox(transform.position, 4f, 1.5f);
            yield return new WaitForSeconds(2f); // Long charge
            DealDamageInArea(transform.position, 4f, 40f);
        }
    
        void ShowHitbox(Vector3 center, float radius, float duration)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = center;
            sphere.transform.localScale = Vector3.one * radius * 2f;
            sphere.GetComponent<Collider>().enabled = false;
            Material mat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            mat.color = new Color(1, 0, 0, 0.3f);
            sphere.GetComponent<MeshRenderer>().material = mat;
            Destroy(sphere, duration);
        }
    
        void DealDamageInArea(Vector3 center, float radius, float damage)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    PlayerMovement playerMovement = hit.GetComponent<PlayerMovement>();
                    PlayerCombat playerCombat = hit.GetComponent<PlayerCombat>();
                    if (playerMovement.isInvincible)
                    {
                        return;
                    }
                    Debug.Log("Player hit!");
                    playerCombat.health -= damage;
                }
            }
        }
}
