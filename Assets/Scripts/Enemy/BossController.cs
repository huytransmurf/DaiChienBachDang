using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Enemy
{
    internal class BossController : MonoBehaviour
    {
        [Header("Combat Settings")]
        public float attackRange = 1.5f;
        public float attackCooldown = 2f;
        public int maxHealth = 100;
        public int attackDamage = 20;
        public float moveSpeed = 2f;

        [Header("Health UI")]
        public Slider healthSlider;
        public Image healthFill;
        public Color healthyColor = Color.green;
        public Color lowHealthColor = Color.red;

        [Header("Continuous Attack Settings")]
        public bool enableContinuousAttack = false;
        public float continuousAttackRate = 0.2f;

        [Header("Detection Settings")]
        public float detectionRange = 5f;
        public LayerMask playerLayer = 1;

        [Header("Attack Detection")]
        public Transform attackPoint;
        public float attackRadius = 1f;

        [Header("Dialogue Settings")]
        public GameObject dialoguePanel;
        public TextMeshProUGUI dialogueText;
        public Button nextButton;
        public string[] dialogueLines =
        {
            "Ngươi dám xâm phạm lãnh thổ của ta!",
            "Ta sẽ không tha thứ cho ngươi!",
            "Chuẩn bị chiến đấu!"
        };

        public enum EnemyState
        {
            Idle,
            ShowingDialogue,
            Running,
            Attack,
            Hurt,
            Death
        }

        [Header("Debug")]
        [SerializeField]
        private EnemyState currentState = EnemyState.Idle;

        // Private variables
        private int currentHealth;
        private Transform player;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private bool hasMetPlayer = false;
        private int currentDialogueIndex = 0;
        private float lastAttackTime;
        private bool isAttacking = false;
        private bool isDead = false;

        // Timers
        private float hurtTimer = 0f;
        private float hurtDuration = 1f;
        private float attackAnimationTimer = 0f;
        private float attackAnimationDuration = 1f;

        // Cached distances
        private float distanceToPlayer;
        private bool playerInDetectionRange;
        private bool playerInAttackRange;

        // Buffer to avoid flickering
        private float attackBufferRange = 0.2f;
        private float stateChangeDelay = 0.1f;
        private float lastStateChangeTime = 0f;
        private bool canAttackImmediately = false;

        // Events
        public System.Action OnBossDeath;

        void Start()
        {
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;

            SetupAttackPoint();
            SetupDialogue();
            SetupHealthUI();
            ChangeState(EnemyState.Idle);
        }

        void SetupAttackPoint()
        {
            if (attackPoint == null)
            {
                GameObject attackPointObj = new GameObject("AttackPoint");
                attackPointObj.transform.SetParent(transform);
                attackPointObj.transform.localPosition = new Vector3(1f, 0f, 0f);
                attackPoint = attackPointObj.transform;
            }
        }

        void SetupHealthUI()
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
            UpdateHealthUI();
        }

        void UpdateHealthUI()
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            if (healthFill != null)
            {
                float healthPercent = (float)currentHealth / maxHealth;
                healthFill.color = Color.Lerp(lowHealthColor, healthyColor, healthPercent);
            }
        }

        void SetupDialogue()
        {
            Debug.Log("SetupDialogue");
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            if (nextButton != null)
            {
                Debug.Log("SetupNextButton");
                nextButton.onClick.AddListener(NextDialogue);
            }
        }

        void Update()
        {
            if (isDead)
                return;

            UpdatePlayerDistance();
            UpdateCurrentState();
            UpdateAnimation();
        }

        void UpdatePlayerDistance()
        {
            if (player == null)
            {
                distanceToPlayer = float.MaxValue;
                playerInDetectionRange = false;
                playerInAttackRange = false;
                return;
            }

            distanceToPlayer = Vector2.Distance(transform.position, player.position);
            playerInDetectionRange = distanceToPlayer <= detectionRange;

            float effectiveAttackRange =
                currentState == EnemyState.Attack ? attackRange + attackBufferRange : attackRange;

            playerInAttackRange = distanceToPlayer <= effectiveAttackRange;
        }

        void UpdateCurrentState()
        {
            HandleCurrentState();

            if (Time.time >= lastStateChangeTime + stateChangeDelay)
            {
                CheckForStateTransition();
            }
        }

        void CheckForStateTransition()
        {
            if (
                currentState == EnemyState.Hurt
                || currentState == EnemyState.ShowingDialogue
                || currentState == EnemyState.Death
            )
            {
                return;
            }

            if (isAttacking)
            {
                return;
            }

            if (!hasMetPlayer && playerInDetectionRange)
            {
                ChangeState(EnemyState.ShowingDialogue);
                return;
            }

            if (!hasMetPlayer)
            {
                return;
            }

            if (playerInAttackRange)
            {
                if (currentState == EnemyState.Running)
                {
                    canAttackImmediately = true;
                }
                ChangeState(EnemyState.Attack);
                return;
            }

            if (playerInDetectionRange)
            {
                ChangeState(EnemyState.Running);
                return;
            }

            ChangeState(EnemyState.Idle);
        }

        void HandleCurrentState()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    HandleIdle();
                    break;
                case EnemyState.ShowingDialogue:
                    HandleDialogue();
                    break;
                case EnemyState.Running:
                    HandleRunning();
                    break;
                case EnemyState.Attack:
                    HandleAttack();
                    break;
                case EnemyState.Hurt:
                    HandleHurt();
                    break;
                case EnemyState.Death:
                    // Do nothing, boss is dead
                    break;
            }
        }

        void HandleIdle()
        {
            if (hasMetPlayer && player != null)
            {
                FacePlayer();
            }
        }

        void HandleDialogue()
        {
            if (player != null)
            {
                FacePlayer();
            }
        }

        void HandleRunning()
        {
            if (player != null)
            {
                FacePlayer();
                Vector2 direction = (player.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }

        void HandleAttack()
        {
            if (player != null)
            {
                FacePlayer();
            }

            if (isAttacking)
            {
                attackAnimationTimer += Time.deltaTime;
                if (attackAnimationTimer >= attackAnimationDuration)
                {
                    isAttacking = false;
                    attackAnimationTimer = 0f;
                    Debug.Log("Attack animation completed");
                }
                return;
            }

            float currentCooldown = enableContinuousAttack ? continuousAttackRate : attackCooldown;
            bool canAttack =
                canAttackImmediately || (Time.time >= lastAttackTime + currentCooldown);

            if (canAttack)
            {
                PerformAttack();
                canAttackImmediately = false;
            }
        }

        void HandleHurt()
        {
            hurtTimer += Time.deltaTime;
            if (hurtTimer >= hurtDuration)
            {
                hurtTimer = 0f;

                if (hasMetPlayer && player != null)
                {
                    if (playerInAttackRange)
                    {
                        ChangeState(EnemyState.Attack);
                    }
                    else if (playerInDetectionRange)
                    {
                        ChangeState(EnemyState.Running);
                    }
                    else
                    {
                        ChangeState(EnemyState.Idle);
                    }
                }
                else
                {
                    ChangeState(EnemyState.Idle);
                }
            }
        }

        void FacePlayer()
        {
            if (player != null)
            {
                bool faceRight = player.position.x > transform.position.x;
                FlipSprite(faceRight);
            }
        }

        void PerformAttack()
        {
            Debug.Log("Boss attacks!");

            isAttacking = true;
            attackAnimationTimer = 0f;
            lastAttackTime = Time.time;

            if (enableContinuousAttack)
            {
                attackAnimationDuration = continuousAttackRate * 0.8f;
            }
            else
            {
                attackAnimationDuration = 1f;
            }

            StartCoroutine(AttackAnimation());

            float damageDelay = enableContinuousAttack ? 0.1f : 0.3f;
            StartCoroutine(DealDamageAfterDelay(damageDelay));
        }

        IEnumerator AttackAnimation()
        {
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;

                if (enableContinuousAttack)
                {
                    spriteRenderer.color = Color.red;
                    yield return new WaitForSeconds(0.1f);
                    spriteRenderer.color = originalColor;
                }
                else
                {
                    spriteRenderer.color = Color.red;
                    yield return new WaitForSeconds(0.2f);
                    spriteRenderer.color = originalColor;
                }
            }
        }

        IEnumerator DealDamageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Use attackPoint and attackRadius for more precise detection
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius,
                playerLayer
            );

            foreach (Collider2D target in hitTargets)
            {
                if (target.CompareTag("Player"))
                {
                    //PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                    //if (playerHealth != null)
                    //{
                    //    playerHealth.TakeDamage(attackDamage);
                    //    Debug.Log($"Boss dealt {attackDamage} damage to player!");
                    //}
                }
            }
        }

        #region Dialogue System
        void ShowDialogue()
        {
            if (dialoguePanel != null && dialogueText != null)
            {
                dialoguePanel.SetActive(true);
                currentDialogueIndex = 0;
                DisplayCurrentDialogue();
            }
        }

        void DisplayCurrentDialogue()
        {
            if (currentDialogueIndex < dialogueLines.Length)
            {
                dialogueText.text = dialogueLines[currentDialogueIndex];
                Debug.Log("Displaying dialogue: " + dialogueLines[currentDialogueIndex]);
            }
        }

        public void NextDialogue()
        {
            Debug.Log("Next dialogue");
            currentDialogueIndex++;

            if (currentDialogueIndex < dialogueLines.Length)
            {
                DisplayCurrentDialogue();
            }
            else
            {
                EndDialogue();
            }
        }

        void EndDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            hasMetPlayer = true;

            if (
                player != null
                && Vector2.Distance(transform.position, player.position) <= attackRange
            )
            {
                canAttackImmediately = true;
                ChangeState(EnemyState.Attack);
            }
            else if (
                player != null
                && Vector2.Distance(transform.position, player.position) <= detectionRange
            )
            {
                ChangeState(EnemyState.Running);
            }
            else
            {
                ChangeState(EnemyState.Idle);
            }
        }
        #endregion

        #region Health System
        public void TakeDamage(int damage)
        {
            if (isDead)
                return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            Debug.Log($"Boss took {damage} damage. Health: {currentHealth}/{maxHealth}");

            UpdateHealthUI();

            if (currentHealth <= 0)
            {
                //Die();
            }
            else
            {
                ChangeState(EnemyState.Hurt);
            }
        }

        void Die()
        {
            Debug.Log("Boss died!");
            isDead = true;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            ChangeState(EnemyState.Death);

            // Trigger death animation
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }

            // Trigger death event
            OnBossDeath?.Invoke();

            // Disable after delay to allow death animation
            StartCoroutine(DisableAfterDelay(2f));
        }

        IEnumerator DisableAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
        #endregion

        #region State Management
        void ChangeState(EnemyState newState)
        {
            if (currentState == newState)
                return;

            OnExitState(currentState);

            EnemyState previousState = currentState;
            currentState = newState;
            lastStateChangeTime = Time.time;
            OnEnterState(newState);

            Debug.Log($"Boss state: {previousState} -> {newState}");
        }

        void OnExitState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.ShowingDialogue:
                    if (dialoguePanel != null)
                    {
                        dialoguePanel.SetActive(false);
                    }
                    break;
            }
        }

        void OnEnterState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Hurt:
                    hurtTimer = 0f;
                    break;
                case EnemyState.ShowingDialogue:
                    ShowDialogue();
                    break;
                case EnemyState.Attack:
                    if (!isAttacking)
                    {
                        attackAnimationTimer = 0f;
                    }
                    break;
            }
        }
        #endregion

        #region Animation & Visual
        void FlipSprite(bool faceRight)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !faceRight;
            }
        }

        void UpdateAnimation()
        {
            if (animator == null)
                return;

            animator.SetBool("isRunning", currentState == EnemyState.Running);
            animator.SetBool("isAttacking", currentState == EnemyState.Attack && isAttacking);
            animator.SetBool("isHurting", currentState == EnemyState.Hurt);
            //animator.SetBool("isDead", currentState == EnemyState.Death);
        }

        // Visualize attack range in editor
        void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Attack point
            if (attackPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
            }
        }

        public void TriggerDialogue()
        {
            if (!hasMetPlayer && currentState == EnemyState.Idle)
            {
                ChangeState(EnemyState.ShowingDialogue);
            }
        }

        [ContextMenu("Force Attack State")]
        public void ForceAttackState()
        {
            ChangeState(EnemyState.Attack);
        }

        // Public getters
        public bool IsDead => isDead;
        public float HealthPercent => (float)currentHealth / maxHealth;
        #endregion
    }
}
