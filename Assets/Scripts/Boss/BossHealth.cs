using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 500;
    private int currentHealth;
    private int maxHealth;

    [Header("Status")]
    public bool isInvulnerable = false;
    private bool isDead = false;

    [Header("UI")]
    public Slider healthSlider; // Gán trong Inspector

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        maxHealth = health;
        currentHealth = health;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(true);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Boss Health: " + currentHealth);

        // ✅ Cập nhật thanh máu
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        // ✅ Animation bị thương
        animator.SetTrigger("hurt");

        // 🔥 Nếu máu yếu, đổi trạng thái animation
        if (currentHealth <= 200)
        {
            animator.SetBool("isFire", true);
        }

        // ☠️ Kiểm tra boss chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // ⚰️ Animation chết
        animator.SetTrigger("die");

        // Tắt va chạm
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // ❌ Ẩn thanh máu
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        // Hủy boss sau delay nếu cần
        Destroy(gameObject, 2f);
    }

    // 👉 Gọi từ Animation Event
    public void DestroyBoss()
    {
        Destroy(gameObject);
    }
}
