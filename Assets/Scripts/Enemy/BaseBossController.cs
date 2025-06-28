using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Player;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public enum BossState
    {
        Idle,
        Moving,
        Attack1,
        Attack2,
        Attack3,
        Jumping,
        Blocking,
        Hurt,
        Dead,
        Circling
    }

    public enum Direction
    {
        Left = -1,
        Right = 1
    }

    [System.Serializable]
    public class BossStats
    {
        [Header("Health & Defense")]
        public float maxHealth = 100f;
        public float blockDamageReduction = 0.5f;
        public float blockDuration = 2f;
        public float blockCooldown = 5f;

        [Header("Movement")]
        public float moveSpeed = 3f;
        public float jumpForce = 6f;
        public float groundCheckDistance = 0.1f;
        public float circlingSpeed = 2f;
        public float minCirclingDistance = 2f;
        public float maxCirclingDistance = 5f;

        [Header("Attack Settings")]
        public float attack1Damage = 10f;
        public float attack1Range = 2f;
        public float attack1Cooldown = 2f;

        public float attack2Damage = 20f;
        public float attack2Range = 3f;
        public float attack2Cooldown = 5f;

        public float attack3Damage = 30f;
        public float attack3Range = 4f;
        public float attack3Cooldown = 8f;

        [Header("AI Behavior")]
        public float detectionRange = 8f;
        public float attackRange = 4.5f;
        public float idleTime = 0.3f;
        public float hurtStunDuration = 0.5f;
        public float aggressionLevel = 1f;

        [Header("Attack Probabilities (0-100)")]
        [Range(0, 100)]
        public int attack1Probability = 40;

        [Range(0, 100)]
        public int attack2Probability = 30;

        [Range(0, 100)]
        public int attack3Probability = 30;

        [Range(0, 100)]
        public int blockProbability = 15;

        [Range(0, 100)]
        public int jumpProbability = 25;

        [Range(0, 100)]
        public int circlingProbability = 30;
    }

    public abstract class BaseBossController : MonoBehaviour
    {
        [Header("Boss Configuration")]
        public BossStats stats;
        public LayerMask groundLayer;
        public LayerMask playerLayer;
        public Transform groundCheck;
        public Transform attackPoint;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip attackSound1;
        public AudioClip attackSound2;
        public AudioClip attackSound3;
        public AudioClip hurtSound;
        public AudioClip jumpSound;
        public AudioClip blockSound;

        // Components
        protected Rigidbody2D rb;
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;
        protected Transform player;

        // State Management
        public BossState CurrentState { get; private set; }
        protected BossState previousState;
        protected float stateTimer;
        protected bool canChangeState = true;

        // Stats
        protected float currentHealth;
        protected Direction facingDirection = Direction.Right;
        protected bool isGrounded;
        protected bool isBlocking;
        protected bool isDead;
        protected bool playerDetected = false;

        // Cooldowns
        //protected float attack1LastTime = -999f;
        //protected float attack2LastTime = -999f;
        //protected float attack3LastTime = -999f;
        //protected float blockLastTime = -999f;
        protected float lastDamageTime;

        // AI Decision Making - SIMPLIFIED
        protected float decisionTimer;
        protected float decisionInterval = 1f; // Tăng interval để ít spam hơn
        protected bool isInAttackMode = false;

        // Circling behavior
        protected float circlingAngle = 0f;
        protected int circlingDirection = 1;
        protected float circlingTimer = 0f;
        protected float maxCirclingTime = 3f;

        // Events
        public Action<float> OnHealthChanged;
        public Action<BossState> OnStateChanged;
        public Action OnDeath;
        public Action<float> OnDamageTaken;

        protected virtual void Awake()
        {
            InitializeComponents();
            InitializeStats();
        }

        protected virtual void Start()
        {
            audioSource = GetComponent<AudioSource>();
            FindPlayer();
        }

        protected virtual void Update()
        {
            if (isDead)
                return;

            UpdateGroundCheck();
            UpdatePlayerDetection();
            UpdateDecisionTimer();
            HandleCurrentState();
            UpdateStateTimer();
        }

        protected virtual void FixedUpdate()
        {
            if (isDead)
                return;
            HandleMovement();
        }

        #region Initialization
        protected virtual void InitializeComponents()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        protected virtual void InitializeStats()
        {
            currentHealth = stats.maxHealth;
            OnHealthChanged?.Invoke(currentHealth / stats.maxHealth);
            circlingDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        }

        protected virtual void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("[Boss] Player not found! Make sure Player has 'Player' tag.");
            }
        }
        #endregion

        #region State Management
        public virtual void ChangeState(BossState newState)
        {
            // Cho phép chuyển sang Hurt hoặc Dead bất cứ lúc nào
            if (!canChangeState && newState != BossState.Hurt && newState != BossState.Dead)
            {
                return;
            }

            previousState = CurrentState;
            CurrentState = newState;
            stateTimer = 0f;

            OnStateChanged?.Invoke(CurrentState);
            OnStateEnter(newState);
            UpdateAnimator();
        }

        protected virtual void OnStateEnter(BossState state)
        {
            switch (state)
            {
                case BossState.Idle:
                    canChangeState = true;
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    break;
                case BossState.Moving:
                    canChangeState = true;
                    isInAttackMode = false; // Rời khỏi attack mode khi di chuyển
                    break;
                case BossState.Circling:
                    canChangeState = true;
                    circlingTimer = 0f;
                    if (UnityEngine.Random.value < 0.3f)
                        circlingDirection *= -1;
                    break;
                case BossState.Attack1:
                    canChangeState = false;
                    isInAttackMode = true;
                    break;
                case BossState.Attack2:
                    canChangeState = false;
                    isInAttackMode = true;
                    break;
                case BossState.Attack3:
                    canChangeState = false;
                    isInAttackMode = true;
                    break;
                case BossState.Jumping:
                    canChangeState = false;
                    PerformJump();
                    break;
                case BossState.Blocking:
                    canChangeState = false;
                    StartCoroutine(PerformBlock());
                    break;
                case BossState.Hurt:
                    canChangeState = false;
                    StartCoroutine(HandleHurt());
                    break;
                case BossState.Dead:
                    canChangeState = false;
                    HandleDeath();
                    break;
            }
        }

        protected virtual void HandleCurrentState()
        {
            switch (CurrentState)
            {
                case BossState.Idle:
                    HandleIdleState();
                    break;
                case BossState.Moving:
                    HandleMovingState();
                    break;
                case BossState.Circling:
                    HandleCirclingState();
                    break;
            }
        }
        #endregion

        #region State Handlers
        protected virtual void HandleIdleState()
        {
            if (stateTimer >= stats.idleTime && canChangeState)
            {
                MakeAIDecision();
            }
        }

        protected virtual void HandleMovingState()
        {
            if (player == null || !canChangeState)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= stats.attackRange)
            {
                isInAttackMode = true;
                MakeAIDecision();
            }
        }

        protected virtual void HandleCirclingState()
        {
            if (player == null || !canChangeState)
                return;

            circlingTimer += Time.deltaTime;

            if (circlingTimer >= maxCirclingTime)
            {
                MakeAIDecision();
            }
        }
        #endregion

        #region AI Decision Making - FIXED LOGIC
        protected virtual void UpdatePlayerDetection()
        {
            if (player == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            playerDetected = distanceToPlayer <= stats.detectionRange;
        }

        protected virtual void UpdateDecisionTimer()
        {
            decisionTimer += Time.deltaTime;
            if (decisionTimer >= decisionInterval && canChangeState)
            {
                decisionTimer = 0f;
                MakeAIDecision();
            }
        }

        protected virtual void MakeAIDecision()
        {
            if (player == null || !canChangeState)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Nếu player ngoài tầm phát hiện -> Idle
            if (!playerDetected)
            {
                if (CurrentState != BossState.Idle)
                {
                    ChangeState(BossState.Idle);
                }
                return;
            }

            // Nếu player xa tầm đánh -> Di chuyển đến gần
            Debug.Log("aaaaa[Boss] Decide movement action: " + distanceToPlayer);
            if (distanceToPlayer > stats.attackRange)
            {
                isInAttackMode = false;
                DecideMovementAction(distanceToPlayer);
            }
            // Nếu player trong tầm đánh -> Spam attacks
            else
            {
                if (isInAttackMode)
                {
                    DecideAttackAction();
                }
                else
                {
                    // Lần đầu vào tầm đánh, bắt đầu attack mode
                    isInAttackMode = true;
                    DecideAttackAction();
                }
            }
        }

        protected virtual void DecideMovementAction(float distanceToPlayer)
        {
            Debug.Log("aaaaa[Boss] Decide movement action");
            if (distanceToPlayer > stats.attackRange * 1.5f)
            {
                if (UnityEngine.Random.value < 0.8f) // 80% chance di chuyển thẳng
                {
                    Debug.Log("bbbbbb[Boss] Decide movement action");
                    ChangeState(BossState.Moving);
                }
                else if (CanJump()) // 20% chance nhảy
                {
                    ChangeState(BossState.Jumping);
                }
                else
                {
                    ChangeState(BossState.Moving);
                }
            }
            else
            {
                // Player gần tầm đánh, có thể circling hoặc di chuyển
                int action = UnityEngine.Random.Range(0, 100);

                if (action < 60) // 60% di chuyển thẳng
                {
                    ChangeState(BossState.Moving);
                }
                else if (action < 80 && CanJump()) // 20% nhảy
                {
                    ChangeState(BossState.Jumping);
                }
                else // 20% circling
                {
                    ChangeState(BossState.Circling);
                }
            }
        }

        protected virtual void DecideAttackAction()
        {
            Debug.Log("[Boss] In attack range - choosing attack");

            // Tạo danh sách các attack có thể sử dụng
            List<System.Action> availableAttacks = new List<System.Action>();

            availableAttacks.Add(TryAttack1);
            availableAttacks.Add(TryAttack2);
            availableAttacks.Add(TryAttack3);

            // Nếu có attack available -> thực hiện
            if (availableAttacks.Count > 0)
            {
                // Random chọn 1 attack
                var selectedAttack = availableAttacks[
                    UnityEngine.Random.Range(0, availableAttacks.Count)
                ];
                selectedAttack.Invoke();
            }
            else
            {
                // Tất cả attack đang cooldown -> có thể block hoặc di chuyển tactical
                int action = UnityEngine.Random.Range(0, 100);

                if (action < 30) // 30% block
                {
                    TryBlock();
                }
                else if (action < 60) // 30% circling
                {
                    ChangeState(BossState.Circling);
                }
                else // 40% idle chờ cooldown
                {
                    ChangeState(BossState.Idle);
                }
            }
        }
        #endregion

        #region Action Methods
        protected virtual void TryAttack1()
        {
            //if (CanUseAttack1())
            //{
            //    attack1LastTime = Time.time;
            ChangeState(BossState.Attack1);
        }

        protected virtual void TryAttack2()
        {
            //if (CanUseAttack2())
            //{
            //    attack2LastTime = Time.time;
            ChangeState(BossState.Attack2);
        }

        protected virtual void TryAttack3()
        {
            //if (CanUseAttack3())
            //{
            //    attack3LastTime = Time.time;
            ChangeState(BossState.Attack3);
        }

        protected virtual void TryBlock()
        {
            //if (CanUseBlock())
            //{
            //    blockLastTime = Time.time;
            ChangeState(BossState.Blocking);
        }
        #endregion

        #region Movement & Physics
        protected virtual void HandleMovement()
        {
            if (player == null || isDead)
                return;

            switch (CurrentState)
            {
                case BossState.Moving:
                    HandleDirectMovement();
                    break;
                case BossState.Circling:
                    HandleCirclingMovement();
                    break;
                case BossState.Idle:
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    break;
            }
        }

        protected virtual void HandleDirectMovement()
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (Mathf.Abs(direction.x) > 0.1f)
            {
                facingDirection = direction.x > 0 ? Direction.Right : Direction.Left;
                rb.linearVelocity = new Vector2(direction.x * stats.moveSpeed, rb.linearVelocity.y);
            }

            UpdateFacing();
        }

        protected virtual void HandleCirclingMovement()
        {
            if (player == null)
                return;

            Vector2 toPlayer = player.position - transform.position;
            float currentDistance = toPlayer.magnitude;

            circlingAngle += stats.circlingSpeed * circlingDirection * Time.fixedDeltaTime;

            Vector2 circlingDir =
                new Vector2(Mathf.Cos(circlingAngle + Mathf.PI / 2), 0) * circlingDirection;

            Vector2 distanceCorrection = Vector2.zero;
            if (currentDistance < stats.minCirclingDistance)
            {
                distanceCorrection = -toPlayer.normalized * 0.5f;
            }
            else if (currentDistance > stats.maxCirclingDistance)
            {
                distanceCorrection = toPlayer.normalized * 0.5f;
            }

            Vector2 finalDirection = (circlingDir + distanceCorrection).normalized;

            if (Mathf.Abs(finalDirection.x) > 0.1f)
            {
                facingDirection = finalDirection.x > 0 ? Direction.Right : Direction.Left;
                rb.linearVelocity = new Vector2(
                    finalDirection.x * stats.circlingSpeed,
                    rb.linearVelocity.y
                );
            }

            UpdateFacing();
        }

        protected virtual void PerformJump()
        {
            if (isGrounded)
            {
                Debug.Log("[Boss] Performing jump");
                PlaySound(jumpSound);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 2f, stats.jumpForce);
                StartCoroutine(WaitForLanding());
            }
            else
            {
                ChangeState(BossState.Moving);
            }
        }

        protected virtual IEnumerator WaitForLanding()
        {
            yield return new WaitForSeconds(0.1f);

            float timeout = 5f;
            float timer = 0f;

            while (!isGrounded && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            canChangeState = true;
            MakeAIDecision();
        }

        protected virtual void UpdateGroundCheck()
        {
            if (groundCheck != null)
            {
                isGrounded = Physics2D.Raycast(
                    groundCheck.position,
                    Vector2.down,
                    stats.groundCheckDistance,
                    groundLayer
                );
            }
            else
            {
                isGrounded = Physics2D.Raycast(
                    transform.position,
                    Vector2.down,
                    stats.groundCheckDistance,
                    groundLayer
                );
            }
        }

        protected virtual void UpdateFacing()
        {
            if (facingDirection == Direction.Right)
                transform.localScale = new Vector3(1f, 1f, 1f);
            else
                transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        protected virtual void FacePlayer()
        {
            if (player != null)
            {
                Vector2 direction = player.position - transform.position;
                facingDirection = direction.x > 0 ? Direction.Right : Direction.Left;
                UpdateFacing();
            }
        }
        #endregion

        #region Cooldown Checks
        //protected virtual bool CanUseAttack1()
        //{
        //    return Time.time >= attack1LastTime + stats.attack1Cooldown;
        //}

        //protected virtual bool CanUseAttack2()
        //{
        //    return Time.time >= attack2LastTime + stats.attack2Cooldown;
        //}

        //protected virtual bool CanUseAttack3()
        //{
        //    return Time.time >= attack3LastTime + stats.attack3Cooldown;
        //}

        //protected virtual bool CanUseBlock()
        //{
        //    return Time.time >= blockLastTime + stats.blockCooldown;
        //}

        protected virtual bool CanJump()
        {
            return isGrounded;
        }
        #endregion

        #region Attack Implementations - GIỮ NGUYÊN
        protected virtual IEnumerator PerformAttack1()
        {
            Debug.Log("Attack 1 executed");
            FacePlayer();
            PlaySound(attackSound1);

            yield return new WaitForSeconds(0.2f);
            DealDamageInRange(stats.attack1Damage, stats.attack1Range);
            OnAttack1();

            yield return new WaitForSeconds(0.3f);

            canChangeState = true;
            // Sau khi attack xong, tiếp tục ở attack mode nếu player vẫn trong tầm
            MakeAIDecision();
        }

        protected virtual IEnumerator PerformAttack2()
        {
            Debug.Log("Attack 2 executed");
            FacePlayer();
            PlaySound(attackSound2);

            yield return new WaitForSeconds(0.4f);
            DealDamageInRange(stats.attack2Damage, stats.attack2Range);
            OnAttack2();

            yield return new WaitForSeconds(0.5f);

            canChangeState = true;
            MakeAIDecision();
        }

        protected virtual IEnumerator PerformAttack3()
        {
            Debug.Log("Attack 3 executed");
            FacePlayer();
            PlaySound(attackSound3);

            yield return new WaitForSeconds(0.6f);
            DealDamageInRange(stats.attack3Damage, stats.attack3Range);
            OnAttack3();

            yield return new WaitForSeconds(0.7f);

            canChangeState = true;
            MakeAIDecision();
        }

        protected virtual void DealDamageInRange(float damage, float range)
        {
            if (player == null)
                return;

            Vector3 attackPosition =
                attackPoint != null ? attackPoint.position : transform.position;
            float distanceToPlayer = Vector2.Distance(attackPosition, player.position);

            if (distanceToPlayer <= range)
            {
                var playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage);
                    Debug.Log($"[Boss] Dealt {damage} damage to player");
                }
            }
        }
        #endregion

        #region Damage & Health
        protected virtual IEnumerator HandleHurt()
        {
            Debug.Log("[Boss] Hurt state");
            OnHurt();
            yield return new WaitForSeconds(stats.hurtStunDuration);

            canChangeState = true;
            MakeAIDecision();
        }

        protected virtual IEnumerator PerformBlock()
        {
            Debug.Log("[Boss] Performing block");
            isBlocking = true;
            PlaySound(blockSound);
            OnBlockStart();

            yield return new WaitForSeconds(stats.blockDuration);

            isBlocking = false;
            OnBlockEnd();

            canChangeState = true;
            MakeAIDecision();
        }

        public virtual void HandleDeath()
        {
            Debug.Log("[Boss] Death state");
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            OnDeath?.Invoke();
            OnDeathCustom();
        }

        public bool IsDead()
        {
            return isDead;
        }
        #endregion

        protected virtual void UpdateAnimator()
        {
            if (animator == null)
                return;

            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsBlocking", isBlocking);

            switch (CurrentState)
            {
                case BossState.Idle:
                    animator.SetTrigger("Idle");
                    break;
                case BossState.Attack1:
                    animator.SetTrigger("Attack1");
                    break;
                case BossState.Attack2:
                    animator.SetTrigger("Attack2");
                    break;
                case BossState.Attack3:
                    animator.SetTrigger("Attack3");
                    break;
                case BossState.Jumping:
                    animator.SetTrigger("Jump");
                    break;
                case BossState.Hurt:
                    animator.SetTrigger("Hurt");
                    break;
                case BossState.Dead:
                    animator.SetTrigger("Death");
                    break;
            }
        }

        protected virtual void PlaySound(AudioClip clip)
        {
            Debug.Log("[Boss] Play sound " + audioSource + " " + clip);
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        protected virtual void UpdateStateTimer()
        {
            stateTimer += Time.deltaTime;
        }

        public virtual void OnAttack1() { }

        protected virtual void OnAttack2() { }

        protected virtual void OnAttack3() { }

        protected virtual void OnBlockStart() { }

        protected virtual void OnBlockEnd() { }

        protected virtual void OnBlockSuccess() { }

        protected virtual void OnHurt() { }

        protected virtual void OnDeathCustom() { }
    }
}
