using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Đã chạm vào " + collision.name);
            PlayerInventory.instance.hasKey = true;
            Debug.Log("Đ? nh?t ch?a khóa!");
            Destroy(gameObject); // Ho?c gameObject.SetActive(false);
        }
    }
}
