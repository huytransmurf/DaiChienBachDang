using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CapsuleCollider2D))]
public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Movement & Follow Settings")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float followDistance = 2f;
    [SerializeField] protected Transform player;

    [Header("Jump Settings")]
    [SerializeField] protected float jumpForce = 10f;
    [SerializeField] protected int maxJumpCount = 2;

    [Header("Attack Settings")]
    [SerializeField] protected float attackDistance = 3f; // Tăng lên 3f để phù hợp với phạm vi lớn
    [SerializeField] protected float attackCooldown = 1f;

    private float lastAttackTime;
    private int lastAttackType = 0;

    protected Animator animator;
    protected Rigidbody2D rb;

    protected bool isGrounded = true;
    protected bool shouldFollowPlayer = false;
    protected float lastPosX;
    protected int jumpCount;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
            Debug.LogWarning($"{gameObject.name} is missing player reference.");
    }

    protected virtual void Update()
    {
        if (shouldFollowPlayer)
        {
            HandleMovement(); // Chỉ gọi khi được phép
        }

        HandleFall(); // vẫn xử lý rơi
    }

    protected virtual void HandleMovement()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > followDistance)
        {
            animator.SetBool("isRunning", true);
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            if ((direction.x > 0 && transform.localScale.x < 0) || (direction.x < 0 && transform.localScale.x > 0))
                Flip();
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    protected virtual void HandleFall()
    {
        if (!isGrounded && rb.linearVelocity.y < 0)
            animator.SetBool("isFalling", true);
        else
            animator.SetBool("isFalling", false);
    }

    protected virtual void HandleJump()
    {
        if (jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("isJumping", true);
            jumpCount--;
            isGrounded = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float distanceToEnemy = Vector2.Distance(transform.position, other.transform.position);
            Debug.Log($"{gameObject.name} detected {other.name} at distance {distanceToEnemy} (attackDistance: {attackDistance})");
            if (distanceToEnemy <= attackDistance)
            {
                TryAttack();
            }
        }

        if (other.CompareTag("Enemmy"))
        {
            StartFollowing(); // Bắt đầu di chuyển về phía player khi player đi vào vùng trigger
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackType++;
        if (lastAttackType > 3) lastAttackType = 1;

        animator.SetTrigger($"Atk{lastAttackType}");
        lastAttackTime = Time.time;
    }

    protected void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public virtual void StartFollowing()
    {
        shouldFollowPlayer = true;
    }

    public virtual void StopFollowing()
    {
        shouldFollowPlayer = false;
        animator.SetBool("isRunning", false);
    }
}