using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int health = 500;
    public bool isInvulnerable = false;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead)
            return;

        health -= damage;

        // Play hurt animation
        animator.SetTrigger("hurt");
        Debug.Log("Boss Health: " + health);
        // Check enraged state
        if (health <= 200)
        {
            animator.SetBool("isFire", true);
        }

        // Check dead
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // Play die animation
        animator.SetTrigger("die");

        // Disable collider and AI
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

    }

    // 👉 Hàm này sẽ được gọi bằng Animation Event cuối animation Die
    public void DestroyBoss()
    {
        Destroy(gameObject);
    }
}
