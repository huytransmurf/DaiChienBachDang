using Assets.Scripts.Player;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float force; // Speed of the arrow
    public float damage;

    private float timer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force; // Adjust speed as needed
        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 180); 
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5f) // Adjust the lifetime of the arrow as needed
        {
            Destroy(gameObject);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy the arrow on hit
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject); // Destroy the arrow on wall hit
        }
    }
}
