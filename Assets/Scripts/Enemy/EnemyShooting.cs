using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float minDistance = 5f; // Minimum distance to shoot

    private float timer;
    private Animator animator;
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(player.transform.position, transform.position);
       
        if (distance < minDistance) // Adjust the distance as needed
        {
            timer += Time.deltaTime;
            if (timer >= 1.05f) // Adjust the shooting interval as needed
            {
                //Shoot();
                timer = 0f;
                animator.SetTrigger("Shoot"); // Trigger the shooting animation
            }
        }

    }
    void Shoot()
    {
        GameObject newBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity);
        
    }
}
