using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        private int maxHealth = 100;
        private int currentHealth;

        private Animator animator;
        private Rigidbody2D rb;
        private Collider2D col;

        public HealthBar healthBar;

        private bool isDead = false;

        public GameObject keyPrefab;
        public GameObject goldPrefab;
        public int maxgold;
        public int mingold;

        void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHeal(maxHealth);

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            Debug.Log(currentHealth);
        }

        public void TakeDamage(int damage)
        {
            if (isDead)
                return;
            animator.SetTrigger("Hurt");
            currentHealth -= damage;
            healthBar.SetHeal(currentHealth);
            Debug.Log(currentHealth);

            if (currentHealth <= 0)
            {
                Die();
                GameManager.Instance.bossDefeated = true;
                DropKey();
               // DropGold();
            }
            else
            {
                // Play hurt animation...
            }
        }

        void DropKey()
        {
            Debug.Log("Đã gọi DropKey");
            Vector3 dropPosition = transform.position + new Vector3(0.5f, 0, 0);
            GameObject key = Instantiate(keyPrefab, dropPosition, Quaternion.identity);
            if (key == null)
            {
                Debug.LogError("Chìa khóa bị null!");
                return;
            }
            Rigidbody2D rb = key.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(new Vector2(0.5f, 2f), ForceMode2D.Impulse); 
            }
        }
        void DropGold()
        {
            int goldCount = UnityEngine.Random.Range(mingold, maxgold + 1);
            Debug.Log("Rơi " + goldCount + " vàng!");
            for (int i = 0; i < goldCount; i++)
            {
                Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, 0);
                GameObject gold = Instantiate(goldPrefab, transform.position + randomOffset, Quaternion.identity);
                Rigidbody2D goldRb = gold.GetComponent<Rigidbody2D>();

                if (goldRb != null)
                {
                    Vector2 force = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(1f, 3f));
                    goldRb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }

        void Die()
        {
            isDead = true;

            // Play death animation
            if (animator != null)
                animator.SetTrigger("Die");

            // Disable physics and collider
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            if (col != null)
                col.enabled = false;
             DropGold();
            // Destroy after delay or call other cleanup
            Destroy(gameObject, 2f); // or use Object Pooling
        }
    }
}
