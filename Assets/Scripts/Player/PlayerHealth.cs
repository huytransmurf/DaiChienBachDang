using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public Animator animator;

        public float maxHealth = 100f;
        public float currentHealth;
        public GameObject gameOverMenu;
        public SpriteRenderer spriteRenderer;

        public HealthBar healthBar;

        // Biến cho hiệu ứng nhấp nháy
        public Color damageColor = Color.red;
        public float flashDuration = 0.1f;

        void Start()
        {
            currentHealth = maxHealth;
            //gameOverMenu.SetActive(false);
            healthBar.SetMaxHeal(currentHealth);

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void TakeDamage(float amount)
        {
            animator.SetTrigger("Hurt");
            Debug.Log("TakeDamage" + amount);
            if (currentHealth <= 0)
            {
                Die();
                Debug.Log("Die" + amount);
                return;
            }

            currentHealth -= amount;
            healthBar.SetHeal(currentHealth);
        }

        void Die()
        {
            //currentHealth = 0;
            //gameOverMenu.SetActive(true);
            animator.SetTrigger("Die");
            SceneManager.LoadSceneAsync(0);
            //Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            currentHealth = 100;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
