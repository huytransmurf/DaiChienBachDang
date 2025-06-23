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
{ // Enum cho các trạng thái của Boss
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
        Circling // Thêm state mới cho việc di chuyển xung quanh player
    }

    // Enum cho hướng di chuyển
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
        public float circlingSpeed = 2f; // Tốc độ di chuyển xung quanh
        public float minCirclingDistance = 2f; // Khoảng cách tối thiểu khi circle
        public float maxCirclingDistance = 5f; // Khoảng cách tối đa khi circle

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
        public float idleTime = 0.3f; // Giảm thời gian idle xuống
        public float hurtStunDuration = 0.5f;
        public float aggressionLevel = 0.7f; // Mức độ hung hăng (0-1)

        [Header("Attack Probabilities (0-100)")]
        [Range(0, 100)]
        public int attack1Probability = 40;

        [Range(0, 100)]
        public int attack2Probability = 30;

        [Range(0, 100)]
        public int attack3Probability = 30;

        [Range(0, 100)]
        public int blockProbability = 15; // Giảm xuống để ưu tiên tấn công

        [Range(0, 100)]
        public int jumpProbability = 25; // Tăng lên để năng động hơn

        [Range(0, 100)]
        public int circlingProbability = 30; // Xác suất di chuyển xung quanh
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
        public AudioClip[] attackSounds;
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
        protected float attack1LastTime = -999f;
        protected float attack2LastTime = -999f;
        protected float attack3LastTime = -999f;
        protected float blockLastTime = -999f;
        protected float lastDamageTime;

        // AI Decision Making - SỬA ĐỔI QUAN TRỌNG
        protected float decisionTimer;
        protected float decisionInterval = 0.5f; // Giảm thời gian để quyết định nhanh hơn
        protected List<System.Action> availableActions;
        protected bool isAggressive = true; // Boss luôn trong trạng thái hung hăng
        private float idleStationaryTimer = 0f;
        private const float maxIdleTimeBeforeJump = 3f;

        // Circling behavior
        protected float circlingAngle = 0f;
        protected int circlingDirection = 1; // 1 hoặc -1
        protected float circlingTimer = 0f;
        protected float maxCirclingTime = 3f;

        // Events
        public System.Action<float> OnHealthChanged;
        public System.Action<BossState> OnStateChanged;
        public System.Action OnDeath;
        public System.Action<float> OnDamageTaken;

        protected virtual void Awake()
        {
            stats.attackRange = 4.5f;
            stats.attack1Probability = 40;
            stats.attack2Probability = 30;
            stats.attack3Probability = 30;
            InitializeComponents();
            InitializeStats();
            InitializeActionList();
        }

        protected virtual void Start()
        {
            FindPlayer();
            //while (!playerDetected)
            //{
                ChangeState(BossState.Idle);
            //}
        }

        protected virtual void Update()
        {
            if (isDead)
                return;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= stats.detectionRange)
            {
                playerDetected = true;
                Debug.Log("[Boss] Player detected - Boss activated! Will chase across entire map!");
            }

            UpdateGroundCheck();
            UpdateStateTimer();
            TrackIdleTooLong();

            // ✅ THAY ĐỔI: Boss luôn luôn thực hiện AI decision
            UpdateDecisionTimer();
            HandleCurrentState();
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

            // Random hóa hướng circling ban đầu
            circlingDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        }

        protected virtual void InitializeActionList()
        {
            availableActions = new List<System.Action>
            {
                TryAttack1,
                TryAttack2,
                TryAttack3,
                TryBlock,
                TryJump,
                TryMove,
                TryCircling // Thêm hành động mới
            };
        }

        protected virtual void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("[Boss] Player found: " + player.name);
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
            Debug.Log($"[Boss] Changing state from {CurrentState} to {newState}");

            // Cho phép chuyển sang Hurt hoặc Dead bất cứ lúc nào
            if (!canChangeState && newState != BossState.Hurt && newState != BossState.Dead)
            {
                Debug.Log($"[Boss] Cannot change state to {newState} - state locked");
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
                    break;
                case BossState.Circling:
                    canChangeState = true;
                    circlingTimer = 0f;
                    // Random hóa hướng circling mới
                    if (UnityEngine.Random.value < 0.3f)
                        circlingDirection *= -1;
                    break;
                case BossState.Attack1:
                    canChangeState = false;
                    StartCoroutine(PerformAttack1());
                    break;
                case BossState.Attack2:
                    canChangeState = false;
                    StartCoroutine(PerformAttack2());
                    break;
                case BossState.Attack3:
                    canChangeState = false;
                    StartCoroutine(PerformAttack3());
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
            // ✅ GIẢM THỜI GIAN IDLE và tự động chuyển sang hành động
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

            // ✅ THAY ĐỔI: Không dừng lại mà tiếp tục hành động
            if (distanceToPlayer <= stats.attackRange)
            {
                Debug.Log("[Boss] In attack range - deciding action");
                // Không dừng lại mà ngay lập tức quyết định hành động tiếp theo
                MakeAIDecision();
            }
        }

        protected virtual void HandleCirclingState()
        {
            if (player == null || !canChangeState)
                return;

            circlingTimer += Time.deltaTime;

            // Chuyển sang hành động khác sau một thời gian
            if (circlingTimer >= maxCirclingTime)
            {
                MakeAIDecision();
            }
        }
        #endregion

        #region AI Decision Making - SỬA ĐỔI QUAN TRỌNG
        protected virtual void UpdateDecisionTimer()
        {
            decisionTimer += Time.deltaTime;
            if (decisionTimer >= decisionInterval)
            {
                decisionTimer = 0f;

                // ✅ Chỉ thực hiện quyết định khi có thể thay đổi state
                if (canChangeState)
                {
                    MakeAIDecision();
                }
            }
        }

        protected virtual void MakeAIDecision()
        {
            if (player == null || !canChangeState)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Nếu player quá xa, di chuyển tới gần
            if (distanceToPlayer > stats.detectionRange)
            {
                TryMove();
                return;
            }

            // ✅ LOGIC MỚI: Boss luôn năng động
            if (distanceToPlayer > stats.attackRange)
            {
                // Player xa -> di chuyển hoặc nhảy
                DecideMovementAction();
            }
            else
            {
                // Player gần -> tấn công hoặc di chuyển tactical
                DecideCombatAction();
            }
        }

        protected virtual void DecideCombatAction()
        {
            // Tạo danh sách weighted actions cho combat
            List<(System.Action action, int weight)> combatActions =
                new List<(System.Action, int)>();

            // Thêm các attack có thể dùng
            if (CanUseAttack1())
                combatActions.Add((TryAttack1, stats.attack1Probability));
            if (CanUseAttack2())
                combatActions.Add((TryAttack2, stats.attack2Probability));
            if (CanUseAttack3())
                combatActions.Add((TryAttack3, stats.attack3Probability));

            // Thêm các hành động tactical
            if (CanUseBlock())
                combatActions.Add((TryBlock, stats.blockProbability));
            if (CanJump())
                combatActions.Add((TryJump, stats.jumpProbability));

            // ✅ QUAN TRỌNG: Luôn có thể circling để tránh đứng yên
            combatActions.Add((TryCircling, stats.circlingProbability));

            // Nếu không có attack nào -> ưu tiên movement
            if (!combatActions.Any(x => x.action.Method.Name.Contains("Attack")))
            {
                combatActions.Add((TryMove, 50));
                combatActions.Add((TryCircling, 30));
                if (CanJump())
                    combatActions.Add((TryJump, 20));
            }

            ExecuteWeightedAction(combatActions);
        }

        protected virtual void DecideMovementAction()
        {
            List<(System.Action action, int weight)> movementActions =
                new List<(System.Action, int)>();

            // Ưu tiên di chuyển thẳng
            movementActions.Add((TryMove, 50));

            // Nhảy để tiếp cận
            if (CanJump())
                movementActions.Add((TryJump, stats.jumpProbability));

            // Circling để positioning
            movementActions.Add((TryCircling, 25));

            ExecuteWeightedAction(movementActions);
        }

        protected virtual void ExecuteWeightedAction(
            List<(System.Action action, int weight)> actions
        )
        {
            if (actions.Count == 0)
            {
                // Fallback
                TryMove();
                return;
            }

            int totalWeight = actions.Sum(x => x.weight);
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var (action, weight) in actions)
            {
                currentWeight += weight;
                if (randomValue < currentWeight)
                {
                    Debug.Log($"[Boss] Executing action: {action.Method.Name}");
                    action.Invoke();
                    return;
                }
            }

            // Fallback
            actions[0].action.Invoke();
        }
        #endregion

        #region Action Methods
        protected virtual void TryAttack1()
        {
            if (CanUseAttack1())
            {
                Debug.Log("[Boss] Executing Attack1");
                attack1LastTime = Time.time;
                ChangeState(BossState.Attack1);
            }
            else
            {
                // Nếu không attack được thì di chuyển
                TryCircling();
            }
        }

        protected virtual void TryAttack2()
        {
            if (CanUseAttack2())
            {
                Debug.Log("[Boss] Executing Attack2");
                attack2LastTime = Time.time;
                ChangeState(BossState.Attack2);
            }
            else
            {
                TryCircling();
            }
        }

        protected virtual void TryAttack3()
        {
            if (CanUseAttack3())
            {
                Debug.Log("[Boss] Executing Attack3");
                attack3LastTime = Time.time;
                ChangeState(BossState.Attack3);
            }
            else
            {
                TryCircling();
            }
        }

        protected virtual void TryBlock()
        {
            if (CanUseBlock())
            {
                Debug.Log("[Boss] Executing Block");
                blockLastTime = Time.time;
                ChangeState(BossState.Blocking);
            }
            else
            {
                TryMove();
            }
        }

        protected virtual void TryJump()
        {
            if (CanJump())
            {
                Debug.Log("[Boss] Executing Jump");
                ChangeState(BossState.Jumping);
            }
            else
            {
                TryMove();
            }
        }

        protected virtual void TryMove()
        {
            Debug.Log("[Boss] Executing Move");
            ChangeState(BossState.Moving);
        }

        // ✅ THÊM ACTION MỚI
        protected virtual void TryCircling()
        {
            Debug.Log("[Boss] Executing Circling");
            ChangeState(BossState.Circling);
        }
        #endregion

        #region Movement & Physics - SỬA ĐỔI QUAN TRỌNG
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
                    // Dừng di chuyển ngang
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

            // Tính toán vị trí circling
            circlingAngle += stats.circlingSpeed * circlingDirection * Time.fixedDeltaTime;

            // Điều chỉnh khoảng cách nếu cần
            float targetDistance = Mathf.Clamp(
                currentDistance,
                stats.minCirclingDistance,
                stats.maxCirclingDistance
            );

            // Tính toán hướng di chuyển cho circling
            Vector2 circlingDir =
                new Vector2(
                    Mathf.Cos(circlingAngle + Mathf.PI / 2),
                    0 // Chỉ di chuyển ngang cho 2D platformer
                ) * circlingDirection;

            // Kết hợp circling với việc duy trì khoảng cách
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
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, stats.jumpForce);

                StartCoroutine(WaitForLanding());
            }
            else
            {
                Debug.Log("[Boss] Cannot jump - not grounded");
                // ✅ Không về Idle mà tiếp tục di chuyển
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

            Debug.Log("[Boss] Landed from jump");
            canChangeState = true;

            // ✅ Sau khi nhảy, tiếp tục hành động thay vì về Idle
            MakeAIDecision();
        }

        protected virtual void TrackIdleTooLong()
        {
            if (CurrentState == BossState.Idle || CurrentState == BossState.Moving)
            {
                // Nếu boss không di chuyển ngang (velocity.x ≈ 0)
                if (Mathf.Abs(rb.linearVelocity.x) < 0.05f)
                {
                    idleStationaryTimer += Time.deltaTime;

                    if (idleStationaryTimer >= maxIdleTimeBeforeJump)
                    {
                        Debug.Log("[Boss AI] Idle too long - performing jump & move");

                        idleStationaryTimer = 0f;

                        if (CanJump())
                        {
                            // Random hướng nhảy
                            facingDirection =
                                UnityEngine.Random.value < 0.5f ? Direction.Left : Direction.Right;
                            UpdateFacing();
                            ChangeState(BossState.Jumping);
                        }
                        else
                        {
                            TryMove(); // Nếu không nhảy được thì di chuyển ngang
                        }
                    }
                }
                else
                {
                    // Đang di chuyển, reset timer
                    idleStationaryTimer = 0f;
                }
            }
            else
            {
                // Không phải Idle/Moving thì reset
                idleStationaryTimer = 0f;
            }
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
        protected virtual bool CanUseAttack1()
        {
            return Time.time >= attack1LastTime + stats.attack1Cooldown;
        }

        protected virtual bool CanUseAttack2()
        {
            return Time.time >= attack2LastTime + stats.attack2Cooldown;
        }

        protected virtual bool CanUseAttack3()
        {
            return Time.time >= attack3LastTime + stats.attack3Cooldown;
        }

        protected virtual bool CanUseBlock()
        {
            return Time.time >= blockLastTime + stats.blockCooldown;
        }

        protected virtual bool CanJump()
        {
            return isGrounded;
        }
        #endregion

        #region Attack Implementations
        protected virtual IEnumerator PerformAttack1()
        {
            Debug.Log("111111");
            FacePlayer();

            PlaySound(GetAttackSound());
            yield return new WaitForSeconds(0.2f);
            DealDamageInRange(stats.attack1Damage, stats.attack1Range);
            OnAttack1();

            yield return new WaitForSeconds(0.3f); // Giảm thời gian chờ

            canChangeState = true;
            // ✅ Sau attack, tiếp tục quyết định hành động
            MakeAIDecision();
        }

        protected virtual IEnumerator PerformAttack2()
        {
            Debug.Log("222222");
            FacePlayer();
            PlaySound(GetAttackSound());

            yield return new WaitForSeconds(0.4f);
            DealDamageInRange(stats.attack2Damage, stats.attack2Range);
            OnAttack2();
            yield return new WaitForSeconds(0.5f); // Giảm thời gian chờ

            canChangeState = true;
            MakeAIDecision();
        }

        protected virtual IEnumerator PerformAttack3()
        {
            Debug.Log("333333");
            FacePlayer();
            PlaySound(GetAttackSound());

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
            {
                Debug.Log("[Boss] Cannot deal damage - player is null");
                return;
            }

            Vector3 attackPosition =
                attackPoint != null ? attackPoint.position : transform.position;
            float distanceToPlayer = Vector2.Distance(attackPosition, player.position);

            Debug.Log(
                $"[Boss] Attack - Distance: {distanceToPlayer:F2}, Range: {range:F2}, Damage: {damage}"
            );

            if (distanceToPlayer <= range)
            {
                Debug.Log("[Boss] Player in range - dealing damage");

                var playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage);
                    Debug.Log($"[Boss] Dealt {damage} damage to player via IHealth");
                }
                else
                {
                    var healthComponent = player.GetComponent<MonoBehaviour>();
                    if (healthComponent != null)
                    {
                        var method = healthComponent.GetType().GetMethod("TakeDamage");
                        if (method != null)
                        {
                            method.Invoke(healthComponent, new object[] { damage });
                            Debug.Log($"[Boss] Dealt {damage} damage to player via reflection");
                        }
                        else
                        {
                            Debug.LogWarning("[Boss] Player has no TakeDamage method");
                        }
                    }
                }
            }
            else
            {
                Debug.Log("[Boss] Player out of attack range");
            }
        }
        #endregion

        #region Damage & Health
        public virtual void TakeDamage(float damage)
        {
            if (isDead)
                return;

            float actualDamage = damage;

            if (isBlocking)
            {
                actualDamage *= (1f - stats.blockDamageReduction);
                OnBlockSuccess();
            }

            currentHealth -= actualDamage;
            lastDamageTime = Time.time;

            OnDamageTaken?.Invoke(actualDamage);
            OnHealthChanged?.Invoke(currentHealth / stats.maxHealth);

            Debug.Log(
                $"[Boss] Took {actualDamage} damage. Health: {currentHealth}/{stats.maxHealth}"
            );

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                ChangeState(BossState.Dead);
            }
            else if (!isBlocking)
            {
                ChangeState(BossState.Hurt);
            }

            PlaySound(hurtSound);
        }

        protected virtual IEnumerator HandleHurt()
        {
            Debug.Log("[Boss] Hurt state");
            OnHurt();
            yield return new WaitForSeconds(stats.hurtStunDuration);

            canChangeState = true;
            // ✅ Sau khi hurt, ngay lập tức quyết định hành động
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

        protected virtual void HandleDeath()
        {
            Debug.Log("[Boss] Death state");
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            OnDeath?.Invoke();
            OnDeathCustom();
        }

        #endregion

        #region Animation
        protected virtual void UpdateAnimator()
        {
            if (animator == null)
                return;

            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsBlocking", isBlocking);

            // Set state triggers
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
        #endregion

        #region Audio
        protected virtual void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        protected virtual AudioClip GetAttackSound()
        {
            if (attackSounds != null && attackSounds.Length > 0)
            {
                return attackSounds[UnityEngine.Random.Range(0, attackSounds.Length)];
            }
            return null;
        }
        #endregion

        #region Utility
        protected virtual void UpdateStateTimer()
        {
            stateTimer += Time.deltaTime;
        }
        #endregion

        #region Virtual Methods for Override
        protected virtual void OnAttack1() { }

        protected virtual void OnAttack2() { }

        protected virtual void OnAttack3() { }

        protected virtual void OnBlockStart() { }

        protected virtual void OnBlockEnd() { }

        protected virtual void OnBlockSuccess() { }

        protected virtual void OnHurt() { }

        protected virtual void OnDeathCustom() { }
        #endregion

        #region Debug
        protected virtual void OnDrawGizmosSelected()
        {
            // Vẽ detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stats.detectionRange);

            // Vẽ attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.attackRange);

            // Vẽ ground check
            Transform checkPoint = groundCheck != null ? groundCheck : transform;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                checkPoint.position,
                checkPoint.position + Vector3.down * stats.groundCheckDistance
            );

            // Vẽ attack ranges
            Vector3 attackPos = attackPoint != null ? attackPoint.position : transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPos, stats.attack1Range);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPos, stats.attack2Range);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPos, stats.attack3Range);
        }
        #endregion
    }

    // Interface cho health system
    //public interface IHealth
    //{
    //    void TakeDamage(float damage);
    //    void Heal(float amount);
    //    float GetCurrentHealth();
    //    float GetMaxHealth();
    //}
}
