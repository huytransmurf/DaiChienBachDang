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
        private bool isDefending = false;

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
            if (isDefending)
            {
                Debug.Log("Blocked Damage!");
                return;
            }

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

        public void SetDefending()
        {
            isDefending = true;
        }

        public void ResetDefending()
        {
            isDefending = false;
        }


        public void AddHealth(int amount)
        {
            float healtotal = 0;
            if (amount == 1)
            {
                healtotal = maxHealth * 0.2f;

            }
            else if (amount == 2)
            {
                healtotal = maxHealth * 0.35f;

            }
            maxHealth += healtotal;
            healthBar.SetMaxHeal(maxHealth);
           // Debug.Log("Tăng máu: " + maxHealth);
        }
        public void Healing(int amount)
        {
            float healAmount = 0;

            if (amount == 1)
            {
                healAmount = currentHealth * 0.2f; // Hồi 20% máu
            }
            else if (amount == 2)
            {
                healAmount = currentHealth * 0.35f; // Hồi 35% máu
            }

            currentHealth += healAmount;

            

            healthBar.SetHeal(currentHealth);

          //  Debug.Log("Tăng máu: " + maxHealth);
          //  Debug.Log($" → Máu hiện tại: {currentHealth}/{maxHealth}");
        }
        public void RestartGame()
        {
            Time.timeScale = 1f;
            currentHealth = 100;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }

        public void HealHealth(int amount)
        {
            currentHealth += amount;
            healthBar.SetHeal(currentHealth);
        }
    }
}
