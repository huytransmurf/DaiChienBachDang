using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public Rigidbody2D rb;
    public AudioPlayer audioPlayer;

    private float movement;
    public float speed = 7f;

    public float jumpForce = 10f;

    private int jumpCount = 0;
    public int maxJumpCount = 2;


    public Transform wallCheck;
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    public bool canMove = true;
    public bool canFlip = true;

    private void Update()
    {
        HandleRun();
        HandleJump();
        HandleCombat();
    }
    private void FixedUpdate()
    {
        //transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * speed;
        rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);

        // new
        if (IsTouchingWall())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y - 0.1f);
        }
        // end - new
    }

    private void HandleJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K))
            && jumpCount < maxJumpCount)
        {
            audioPlayer.PlayJumpAudio();
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
            jumpCount++;
        }

        if (IsGrounded() && rb.linearVelocity.y <= 0.1f)
        {
            jumpCount = 0;
            animator.SetBool("isJumping", false);
        }
    }

    private void HandleRun()
    {
        if (!canMove)
        {
            movement = 0f;
            animator.SetFloat("Run", 0f);
            return;
        }

        movement = Input.GetAxis("Horizontal");

        if (Mathf.Abs(movement) > .1f)
        {
            animator.SetFloat("Run", 1f);
            //audioPlayer.PlayRunAudio();
        }
        else
        {
            animator.SetFloat("Run", 0f);
        }
        HandleFlip();
    }

    private void HandleFlip()
    {
        if (!canFlip) return;

        if (movement < 0f)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }

        if (movement > 0f)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }
    private void ResetFlip()
    {
        canFlip = true;
    }

    private void HandleCombat()
    {
        canMove = !(Input.GetKey(KeyCode.S) && IsGrounded());

        if (IsInAttackState()) return;

        HandleUltimateRelease();    // Khi thả nút K (I)

        if(HandleDefendState()) return;        // Bật/tắt phòng thủ (S)

        if (Input.GetKey(KeyCode.W) && IsGrounded())
        {
            HandleSkill2();         // S + J
            HandleCharge();         // S + I
        }
        else
        {
            HandleNormalAttack();   // J hoặc click chuột trái
            HandleSkill1();         // I hoặc click chuột phải
        }
    }

    private void HandleNormalAttack()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack0");
        }
    }

    private void HandleSkill1()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Attack1");
        }
    }

    private void HandleSkill2()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
        {
            if (IsInChargeState()) return;

            canFlip = false;
            animator.SetTrigger("Attack2");
            Invoke(nameof(ResetFlip), 1.0f);
        }
    }

    private void HandleCharge()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetMouseButtonDown(1))
        {
            if (IsInChargeState()) return;

            animator.SetTrigger("Charge");
        }
    }

    private void HandleUltimateRelease()
    {
        if (Input.GetKeyUp(KeyCode.I) || Input.GetMouseButtonUp(1))
        {
            if (IsInChargeState())
            {
                canFlip = false;
                animator.SetTrigger("Ultimate");
                Invoke(nameof(ResetFlip), 1.5f);
            }
        }
    }

    private bool HandleDefendState()
    {
        if (!canMove)
        {
            animator.SetBool("isDefending", true);
            return true;
        }

        animator.SetBool("isDefending", false);
        return false;
    }

    private bool IsInAttackState()
    {
        return
            animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") ||
            animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack");
    }

    private bool IsInChargeState()
    {
        return animator.GetCurrentAnimatorStateInfo(1).IsTag("Charge");
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void PlayAttackAudio()
    {
        audioPlayer.PlayAttackAudio();
    }

    private void PlayJumpAudio()
    {
        audioPlayer.PlayJumpAudio();
    }

}
