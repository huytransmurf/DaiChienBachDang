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

            currentHealth -= damage;
           healthBar.SetHeal(currentHealth);
            Debug.Log(currentHealth);

            if (currentHealth <= 0)
            {
                Die();
                GameManager.instance.bossDefeated = true;
                DropKey();
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
                Debug.LogError("Chìa khóa bị null! Bạn đã gán đúng Prefab chưa?");
                return;
            }
            Rigidbody2D rb = key.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(new Vector2(0.5f, 2f), ForceMode2D.Impulse); // Bay nhẹ sang phải và lên
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

            // Destroy after delay or call other cleanup
            Destroy(gameObject, 2f); // or use Object Pooling
        }
    }
}
