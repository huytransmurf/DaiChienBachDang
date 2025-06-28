using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcMinigame : MonoBehaviour
{
    [System.Serializable]
    public class DialogLine
    {
        public string text;
        public Sprite image;
    }

    [Header("UI Components")]
    public GameObject dialogUI;
    public TextMeshProUGUI dialogText;
    public Image dialogImage;

    [Header("Dialog Content")]
    public DialogLine[] dialogLines;

    [Header("NPC Settings")]
    public GameObject npcToHide; 

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool dialogActive = false;

    public GameObject minigame;
    public static NpcMinigame instance;
    public bool isdonetalk = false;

    void Update()
    {
        if (playerInRange )
        {
            if (!dialogActive && Input.GetKeyDown(KeyCode.F))
            {
                StartDialog();
            }
            else if (dialogActive && Input.GetKeyDown(KeyCode.Tab))
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

        if (GameManager.instance != null)
        {
            GameManager.instance.hasTalkedToNpc = true;
        }

        if (npcToHide != null)
        {
            npcToHide.SetActive(false);
        }

        if(minigame != null)
        {
            isdonetalk = true;
           // minigame.SetActive(true);
            
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            dialogUI.SetActive(false);
            dialogActive = false;
            currentLine = 0;
        }
    }
}
