using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    private bool hasOpened = false;
    private Animator animator;
    private Collider2D doorCollider;

    [Header("Camera Settings")]
    public float focusDuration = 2f;
    public Vector3 cameraOffset = new Vector3(0, 0, -10f);

    private bool playerInRange = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!playerInRange || hasOpened) return;

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            if (PlayerInventory.instance != null && PlayerInventory.instance.hasKey)
            {
                hasOpened = true;
                OpenDoor();
            }
            else
            {
                Debug.Log("Bạn chưa có chìa khóa!");
            }
        }
    }

    void OpenDoor()
    {
        Debug.Log("Cửa mở!");

        if (animator != null)
        {
            animator.SetTrigger("open");
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        FocusCamera();
    }

    void FocusCamera()
    {
        if (Camera.main != null)
        {
            Camera.main.transform.position = transform.position + cameraOffset;
            Invoke("ReturnCameraToPlayer", focusDuration);
        }
    }

    void ReturnCameraToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Camera.main.transform.position = player.transform.position + new Vector3(0, 0, -10f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player đứng gần cửa (Collision).");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
