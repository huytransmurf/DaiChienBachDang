using UnityEngine;

public class WoodPickup : MonoBehaviour
{
    public int woodAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerWood playerWood = collision.GetComponentInParent<PlayerWood>(); // 🔧 Sửa tại đây
            if (playerWood != null)
            {
                playerWood.AddWood(woodAmount);
                Debug.Log("Đã nhặt được gỗ!");
                Destroy(gameObject); // Hoặc SetActive(false);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy PlayerWood trên Player!");
            }
        }
    }
}
