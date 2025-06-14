using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    private bool hasOpened = false;
    private Animator animator;
    private Collider2D doorCollider;

    [Header("Camera Settings")]
    public float focusDuration = 2f;
    public Vector3 cameraOffset = new Vector3(0, 0, -10f); // giữ nguyên Z

    private void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (hasOpened) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            hasOpened = true;
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        Debug.Log("All enemies defeated. Door opening...");

        if (animator != null)
        {
            animator.SetTrigger("open");
        }

        // Disable collider (nếu có)
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        // Focus camera vào cửa
        FocusCamera();

        // Nếu muốn ẩn cửa sau khi mở, dùng dòng này (nếu không thì bỏ)
        // gameObject.SetActive(false);
    }

    void FocusCamera()
    {
        if (Camera.main != null)
        {
            Camera.main.transform.position = transform.position + cameraOffset;

            // Nếu muốn camera focus trong vài giây rồi quay lại player
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
}
