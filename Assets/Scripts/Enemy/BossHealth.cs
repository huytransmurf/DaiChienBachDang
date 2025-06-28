using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public abstract class BossHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public int maxHealth = 100;
        protected int currentHealth;

        protected Animator animator;
        protected Rigidbody2D rb;
        protected Collider2D col;

        public HealthBar healthBar;
        public GameObject mapPrefab;

        protected bool isDead = false;

        public GameObject keyPrefab;
        public GameObject goldPrefab;
        public int maxgold;
        public int mingold;

        public GameObject healthPotionPrefab;
        public float healthPotionPercent;

        void Start()
        {
            currentHealth = maxHealth;
            healthBar.SetMaxHeal(maxHealth);

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            Debug.Log(currentHealth);
        }

        public virtual void TakeDamage(int damage) { }

        public void DropHealthPotion()
        {
            float chance = UnityEngine.Random.value; // random từ 0.0f -> 1.0f
            if (chance <= healthPotionPercent)
            {
                Debug.Log("Rơi bình máu");
                Vector3 dropPosition = transform.position + new Vector3(-0.5f, 0, 0); // lệch nhẹ so với key
                GameObject potion = Instantiate(
                    healthPotionPrefab,
                    dropPosition,
                    Quaternion.identity
                );
                Rigidbody2D rb = potion.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(new Vector2(-0.5f, 2f), ForceMode2D.Impulse);
                }
            }
            else
            {
                //  Debug.Log("Không rơi bình máu");
            }
        }

        public void DropMap()
        {
            Debug.Log("Đã gọi Map");
            Vector3 dropPosition = transform.position + new Vector3(0.5f, 0, 0);
            GameObject map = Instantiate(mapPrefab, dropPosition, Quaternion.identity);
            if (map == null)
            {
                Debug.LogError("Chìa khóa bị null!");
                return;
            }
            Rigidbody2D rb = map.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(new Vector2(0.5f, 2f), ForceMode2D.Impulse);
            }
        }
        public void DropKey()
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

        public void DropGold()
        {
            int goldCount = UnityEngine.Random.Range(mingold, maxgold + 1);
            Debug.Log("Rơi " + goldCount + " vàng!");
            for (int i = 0; i < goldCount; i++)
            {
                Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, 0);
                GameObject gold = Instantiate(
                    goldPrefab,
                    transform.position + randomOffset,
                    Quaternion.identity
                );
                Rigidbody2D goldRb = gold.GetComponent<Rigidbody2D>();

                if (goldRb != null)
                {
                    Vector2 force = new Vector2(
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(1f, 3f)
                    );
                    goldRb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }

        public void Die()
        {
            if (isDead)
                return;

            isDead = true;

            animator.ResetTrigger("attack"); // ← CHẶN animation tấn công lỡ đang active
            animator.SetTrigger("Die");
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = false;
            }

            GameManager.Instance.bossDefeated = true;

            if (GetComponent<Collider2D>() != null)
                GetComponent<Collider2D>().enabled = false;
            DropGold();
            DropHealthPotion();
            Destroy(gameObject, 2f);
        }
    }
}
