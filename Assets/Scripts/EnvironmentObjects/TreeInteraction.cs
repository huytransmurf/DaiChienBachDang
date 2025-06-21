using TMPro;
using UnityEngine;

public class TreeInteraction : MonoBehaviour
{
    public GameObject woodPrefab;
    public Transform interactionPoint;
    public float interactRange = 2f;

    private Transform player;
    private int logsDropped = 0;
    public int maxLogs;
    public GameObject dialogBox;               // Gán DialogBox
    public TextMeshProUGUI dialogText;
    private float messageDuration = 2f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        maxLogs = Random.Range(1, 6);
        if (dialogBox != null)
            dialogBox.SetActive(false);
    }

    void Update()
    {
       if (!GameManager.instance.hasTalkedToNpc || !GameManager.instance.bossDefeated) return;
        if (player == null || interactionPoint == null) return;

        float distance = Vector2.Distance(interactionPoint.position, player.position);
        if (distance <= interactRange && Input.GetMouseButtonDown(0))
        {
            DropWood();
        }
    }

    void DropWood()
    {
        if (logsDropped >= maxLogs)
        {
            ShowDialog("Cây này đã hết gỗ! Bạn hãy đi cây khác");
            return;
        }

        float offsetX = Random.Range(-1f, 1f);
        float offsetY = Random.Range(0.5f, 1.5f);
        Vector3 dropPosition = transform.position + new Vector3(offsetX, offsetY, 0);

        Instantiate(woodPrefab, dropPosition, Quaternion.identity);
        logsDropped++;

        Debug.Log($"Đã chặt ra {logsDropped}/{maxLogs} khúc gỗ.");
    }
    void ShowDialog(string message)
    {
        if (dialogBox != null && dialogText != null)
        {
            dialogText.text = message;
            dialogBox.SetActive(true);
            CancelInvoke(nameof(HideDialog));
            Invoke(nameof(HideDialog), messageDuration);
        }
    }
        
    void HideDialog()
    {
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
    }
}
