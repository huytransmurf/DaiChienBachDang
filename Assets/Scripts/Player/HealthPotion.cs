using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddPotion(amount); // tích lũy bình máu + tự cập nhật UI
            }

            Destroy(gameObject);
        }
    }
}
