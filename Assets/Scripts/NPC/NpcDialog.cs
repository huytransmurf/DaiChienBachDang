using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NpcDialog : MonoBehaviour
{
    [System.Serializable]
    public class DialogLine
    {
        public string text;
        public Sprite image;
    }

    public GameObject dialogUI;                    // Gắn Panel UI trong Inspector
    public TextMeshProUGUI dialogText;             // Gắn TextMeshPro vào
    public Image dialogImage;                      // Gắn Image UI để hiển thị ảnh
    public DialogLine[] dialogLines;               // Mỗi đoạn thoại gồm text + ảnh

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool dialogActive = false;

    public GameObject npcToHide;

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
        ShowLine();
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < dialogLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialog();
        }
    }

    void ShowLine()
    {
        dialogText.text = dialogLines[currentLine].text;

        if (dialogLines[currentLine].image != null)
        {
            dialogImage.sprite = dialogLines[currentLine].image;
            dialogImage.gameObject.SetActive(true);
        }
        else
        {
            dialogImage.gameObject.SetActive(false); // Ẩn nếu không có ảnh
        }
    }

    void EndDialog()
    {
        dialogActive = false;
        dialogUI.SetActive(false);
        currentLine = 0;

        // Nếu có GameManager thì gán trạng thái
        if (GameManager.instance != null)
        {
            GameManager.instance.hasTalkedToNpc = true;
        }

        // Ẩn NPC nếu đã gán trong Inspector
        if (npcToHide != null)
        {
            npcToHide.SetActive(false);
        }
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
            EndDialog(); // Tắt UI khi rời xa
        }
    }
}
