using System;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 2f;
        public float patrolDistance = 1.5f;

        [Header("Combat Settings")]
        public int damage = 100;
        public float attackRange = 1.5f;
        public float attackCooldown = 2f;
        public LayerMask playerLayer;

        [Header("Detection Settings")]
        public float detectionRange = 3f;

        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private Vector2 startPosition;
        private bool movingRight = true;
        private float lastAttackTime;
        private float lastFlipTime;

        private Transform player;
        private bool playerDetected = false;
        public AudioClip attackSound;
        public AudioSource audioSource;

        public bool isDead = false;

        private enum EnemyState
        {
            Idle,
            Patrol,
            Chase,
            Attack
        }

        private EnemyState currentState = EnemyState.Idle;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();

            startPosition = transform.position;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        void Update()
        {
            if (isDead)
                return;
            DetectPlayer();
            StateMachine();
            UpdateAnimation();
        }

        void DetectPlayer()
        {
            if (player == null)
                return;
            float distance = Vector2.Distance(transform.position, player.position);
            playerDetected = distance <= detectionRange;
        }

        public void Die()
        {
            if (isDead)
                return;
            isDead = true;
        }

        public bool IsDead()
        {
            return isDead;
        }

        void StateMachine()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    rb.linearVelocity = Vector2.zero;
                    if (playerDetected)
                        currentState = EnemyState.Chase;
                    else
                        Invoke("StartPatrol", 1f);
                    break;

                case EnemyState.Patrol:
                    if (playerDetected)
                    {
                        currentState = EnemyState.Chase;
                        return;
                    }
                    Patrol();
                    break;

                case EnemyState.Chase:
                    if (!playerDetected)
                    {
                        currentState = EnemyState.Idle;
                        return;
                    }

                    float distance = Vector2.Distance(transform.position, player.position);
                    if (distance <= attackRange)
                        currentState = EnemyState.Attack;
                    else
                        ChasePlayer();
                    break;

                case EnemyState.Attack:
                    rb.linearVelocity = Vector2.zero;

                    if (!playerDetected)
                    {
                        currentState = EnemyState.Idle;
                        return;
                    }

                    float dist = Vector2.Distance(transform.position, player.position);
                    if (dist > attackRange)
                        currentState = EnemyState.Chase;
                    else if (Time.time >= lastAttackTime + attackCooldown)
                        Attack();
                    break;
            }
        }

        void StartPatrol()
        {
            if (currentState == EnemyState.Idle)
                currentState = EnemyState.Patrol;
        }

        void Patrol()
        {
            // Tính toán điểm biên trái và phải
            float leftBound = startPosition.x - patrolDistance;
            float rightBound = startPosition.x + patrolDistance;

            // Kiểm tra có cần flip không
            if (movingRight && transform.position.x >= rightBound)
            {
                movingRight = false;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            else if (!movingRight && transform.position.x <= leftBound)
            {
                movingRight = true;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            // Di chuyển
            float dir = movingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        }

        void ChasePlayer()
        {
            if (player == null)
                return;

            float directionToPlayer = player.position.x - transform.position.x;

            // Chỉ thay đổi hướng khi khoảng cách đủ lớn VÀ đã đủ thời gian từ lần flip trước
            if (Mathf.Abs(directionToPlayer) > 0.3f && Time.time - lastFlipTime > 0.5f)
            {
                bool shouldMoveRight = directionToPlayer > 0;

                if (shouldMoveRight != movingRight)
                {
                    Flip();
                    lastFlipTime = Time.time; // Ghi nhận thời gian flip
                }
            }

            // Luôn di chuyển theo hướng hiện tại
            float moveDir = movingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(moveDir * moveSpeed * 1.5f, rb.linearVelocity.y);
        }

        void Attack()
        {
            lastAttackTime = Time.time;
            Debug.Log("Attack!");

            animator.SetTrigger("attack"); // Kích hoạt trigger

            if (audioSource != null && attackSound != null)
                audioSource.PlayOneShot(attackSound);
        }

        void TakeDamage()
        {
            Collider2D playerCol = Physics2D.OverlapCircle(
                transform.position,
                attackRange,
                playerLayer
            );
            if (playerCol != null)
            {
                Debug.Log("Hit player!");
                PlayerHealth ph = playerCol.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    Debug.Log("Hit player2!");
                    ph.TakeDamage(damage);
                }
            }
        }

        void Flip()
        {
            movingRight = !movingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        void UpdateAnimation()
        {
            if (animator == null)
                return;
            animator.SetBool("isMoving", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
            //animator.SetBool("isAttacking", currentState == EnemyState.Attack);
            animator.SetBool("isChasing", currentState == EnemyState.Chase);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.blue;
            Vector2 startPos = Application.isPlaying ? startPosition : (Vector2)transform.position;
            Gizmos.DrawLine(
                startPos + Vector2.left * patrolDistance,
                startPos + Vector2.right * patrolDistance
            );
        }
    }
}
