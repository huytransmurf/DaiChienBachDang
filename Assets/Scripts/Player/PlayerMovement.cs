using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public Rigidbody2D rb;

    private float movement;
    public float speed = 7f;
    private bool faceRight = true;

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
        if (movement < 0f && faceRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            faceRight = false;
        }
        if (movement > 0f && !faceRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            faceRight = true;
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
        }
        else if (movement < .1f)
        {
            animator.SetFloat("Run", 0f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack_1");
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
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            //animator.SetBool("Jump", false);
            jumpCount = 0;
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
