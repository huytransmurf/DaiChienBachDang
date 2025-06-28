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

    public GameObject dialogUI;                    
    public TextMeshProUGUI dialogText;             
    public Image dialogImage;                      
    public DialogLine[] dialogLines;               

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool dialogActive = false;

    public GameObject npcToHide;
    public GameObject choiceUI;   
    public GameObject shopUI;

    void Update()
    {
        if (playerInRange)
        {
            if (!dialogActive && Input.GetKeyDown(KeyCode.F))
            {
                ShowChoices();
                //StartDialog();
            }
            else if (dialogActive && Input.GetKeyDown(KeyCode.F))
            {
                NextLine();
            }
        }
    }
    void ShowChoices()
    {
        choiceUI.SetActive(true);  // Hiện 2 nút
    }

    public void OnTalkButton()
    {
        choiceUI.SetActive(false);
        StartDialog(); // Bắt đầu đối thoại như cũ
    }

    public void OnShopButton()
    {
        choiceUI.SetActive(false);
        shopUI.SetActive(true); // Mở cửa hàng
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
        shopUI.SetActive(false); // Đóng cửa hàng nếu đang mở
        currentLine = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.hasTalkedToNpc = true;
        }

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
            dialogUI.SetActive(false);
            shopUI.SetActive(false);
            choiceUI.SetActive(false);
            dialogActive = false;
        }
    }
}
