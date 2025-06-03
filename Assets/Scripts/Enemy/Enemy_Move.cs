using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    public float moveDistance = 3f;   // Khoảng cách di chuyển theo trục X
    public float moveSpeed = 2f;      // Tốc độ di chuyển

    private float startX;             // Vị trí X ban đầu
    private bool movingRight = true; // Hướng di chuyển hiện tại
    private bool faceRight = true;   // Đã xoay mặt sang phải chưa

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        float direction = movingRight ? 1f : -1f;
        float newX = transform.position.x + direction * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Xoay mặt theo hướng di chuyển
        if (direction < 0f && faceRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            faceRight = false;
        }
        else if (direction > 0f && !faceRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            faceRight = true;
        }

        // Đảo hướng khi tới giới hạn
        if (movingRight && newX >= startX + moveDistance)
        {
            movingRight = false;
        }
        else if (!movingRight && newX <= startX - moveDistance)
        {
            movingRight = true;
        }
    }
}
