using UnityEngine;
using TMPro;

public class NpcDialog : MonoBehaviour
{
    public GameObject dialogUI;               // Gắn Panel UI trong Inspector
    public TextMeshProUGUI dialogText;        // Gắn TextMeshPro vào
    public string[] dialogLines;              // Nhập hội thoại từng dòng
    private int currentLine = 0;
    private bool playerInRange = false;
    private bool dialogActive = false;

    void Update()
    {
        if (playerInRange)
        {
            if (!dialogActive && Input.GetKeyDown(KeyCode.F))
            {
                StartDialog();
            }
            else if (dialogActive && Input.GetKeyDown(KeyCode.F))
            {
                NextLine();
            }
        }
    }

    void StartDialog()
    {
        dialogActive = true;
        currentLine = 0;
        dialogUI.SetActive(true);
        dialogText.text = dialogLines[currentLine];
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < dialogLines.Length)
        {
            dialogText.text = dialogLines[currentLine];
        }
        else
        {
            EndDialog();
        }
    }

    void EndDialog()
    {
        dialogActive = false;
        dialogUI.SetActive(false);
        currentLine = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            EndDialog(); // tự tắt khi rời xa
        }
    }
}
