using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    public float speed ;
    public float attackRange;

    Transform player;
    Rigidbody2D rb;
    Boss boss;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Speed = " + speed); // Xem có log ra không
        boss.LookAtPlayer();

        float distance = Vector2.Distance(rb.position, player.position);

        if (distance > attackRange)
        {
            // Di chuyển về phía player nếu ngoài tầm đánh
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            // Đã vào tầm thì trigger attack
            animator.SetTrigger("attack1");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("attack1");
    }
}
