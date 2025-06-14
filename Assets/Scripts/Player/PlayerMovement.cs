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

    // new
    public Transform wallCheck;
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    // end - new

    void Update()
    {
        // Di chuyển ngang
        movement = Input.GetAxis("Horizontal");

        // Xoay mặt
        if (movement < 0f)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        if (movement > 0f)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            Jump();
            //animator.SetBool("Jump", true);
            jumpCount++;
        }

        if (Mathf.Abs(movement) > .1f)
        {

            animator.SetFloat("Run", 1f);
            // audioPlayer.PlayRunAudio();

        }
        else if (movement < .1f)
        {
            animator.SetFloat("Run", 0f);


        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack_1");
            audioPlayer.PlayAttackAudio();

        }
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

    void Jump()
    {
        audioPlayer.PlayJumpAudio();
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        animator.SetBool("isJumping", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Nếu normal của điểm tiếp xúc gần với hướng lên trên (Vector2.up)
                if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
                {
                    // animator.SetBool("Jump", false);
                    jumpCount = 0;
                    break; // Thoát vòng lặp khi tìm thấy điểm tiếp xúc hợp lệ
                }
            }
            animator.SetBool("isJumping", false);
        }
    }

    // new
    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }
    // end-new
}
