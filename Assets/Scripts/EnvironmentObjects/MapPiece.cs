using UnityEngine;

public class MapPiece : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory.instance.AddMapPiece();
            Destroy(gameObject);
        }
    }
}
