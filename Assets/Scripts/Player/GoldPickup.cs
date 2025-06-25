using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int goldValue = 1; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player đã nhặt vàng: +" + goldValue);
            PlayerInventory.instance.AddGold(goldValue);
            Destroy(gameObject);
        }
    }
}
