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

    [Header("UI")]
    public GameObject dialogUI;
    public TextMeshProUGUI dialogText;
    public Image dialogImage;

    [Header("Thoại nhiệm vụ")]
    public DialogLine[] beforeCollectingWoodLines;
    public DialogLine[] afterCollectingWoodLines;

    [Header("Tương tác")]
    public GameObject choiceUI;
    public GameObject shopUI;
    public GameObject npcToHide;

    [Header("Phần thưởng nhiệm vụ")]
    public bool requiresWood = false;
    public int requiredWood = 20;
    public GameObject mapPiecePrefab;
    public float dropForce = 3f;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool dialogActive = false;
    private bool rewardGiven = false;

    private bool hasInteracted = false;

    private bool hasCompletedTask = false;

    private DialogLine[] currentDialogLines;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // Nếu chưa từng mở Option => mở lần đầu
            if (!hasInteracted)
            {
                ShowChoices();
                hasInteracted = true;
            }
            else if (hasInteracted)
            {
               // ShowChoices();

                hasInteracted = false;
            }
        }

        if (dialogActive && Input.GetKeyDown(KeyCode.Tab))
        {
            NextLine();
        }
    }

    void ShowChoices()
    {
        if (choiceUI != null)
            choiceUI.SetActive(true);
    }

    public void OnTalkButton()
    {
        if (choiceUI != null)
            choiceUI.SetActive(false);

        StartDialog(); // Bắt đầu thoại
    }


    public void OnShopButton()
    {
        if (choiceUI != null)
            choiceUI.SetActive(false);
        if (shopUI != null)
            shopUI.SetActive(true);
    }

    void StartDialog()
    {
        dialogActive = true;
        currentLine = 0;

        // Cập nhật trạng thái hoàn thành nhiệm vụ
        hasCompletedTask = (requiresWood && PlayerWood.woodCount >= requiredWood);

        // Chọn đoạn thoại đúng theo trạng thái
        if (hasCompletedTask)
            currentDialogLines = afterCollectingWoodLines;
        else
            currentDialogLines = beforeCollectingWoodLines;

        if (dialogUI != null)
            dialogUI.SetActive(true);

        ShowLine();
    }


    void NextLine()
    {
        currentLine++;
        if (currentDialogLines != null && currentLine < currentDialogLines.Length)
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
        if (currentDialogLines == null || currentLine >= currentDialogLines.Length)
            return;

        dialogText.text = currentDialogLines[currentLine].text;

        if (currentDialogLines[currentLine].image != null)
        {
            dialogImage.sprite = currentDialogLines[currentLine].image;
            dialogImage.gameObject.SetActive(true);
        }
        else
        {
            dialogImage.gameObject.SetActive(false);
        }
    }

    void EndDialog()
    {
        dialogActive = false;

        if (dialogUI != null)
            dialogUI.SetActive(false);
        if (shopUI != null)
            shopUI.SetActive(false);

        currentLine = 0;


        // 🎁 Chỉ rơi phần thưởng nếu hoàn thành nhiệm vụ và chưa nhận
        if (hasCompletedTask && !rewardGiven && mapPiecePrefab != null)
        {
            rewardGiven = true;

            Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Vector3 spawnPos = transform.position + (Vector3)offset;

            GameObject mapPiece = Instantiate(mapPiecePrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = mapPiece.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
            }

        }

        // Ẩn NPC nếu cần
        if (npcToHide != null)
            npcToHide.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.hasTalkedToNpc = true;
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

            if (dialogUI != null)
                dialogUI.SetActive(false);
            if (shopUI != null)
                shopUI.SetActive(false);
            if (choiceUI != null)
                choiceUI.SetActive(false);

            dialogActive = false;

            // ✅ Khi rời khỏi vùng NPC, cho phép hiện lại Option lần sau
            hasInteracted = false;
        }
    }

}
