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
        public int maxHealth = 100;
        private int currentHealth;

        private Animator animator;
        private Rigidbody2D rb;
        private Collider2D col;

        public HealthBar healthBar;


        private bool isDead = false;

        void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHeal(currentHealth);

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
        }

        public void TakeDamage(int damage)
        {
            Debug.Log("TakeDamage boss");
            if (isDead)
                return;

            currentHealth -= damage;
            healthBar.SetHeal(currentHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // Play hurt animation...
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
