using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PBHDialog : MonoBehaviour
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

    [Header("Dialog")]
    public DialogLine[] dialogLines;
    public float delayBetweenLines = 2.5f;

    [Header("NPC")]
    public GameObject npcToHide;

    [Header("Phần thưởng")]
    public GameObject mapPiecePrefab;
    public float dropForce = 3f;

    private bool playerInRange = false;
    private bool hasTalked = false;
    private bool isPlayingDialog = false;
    private int currentLine = 0;

    private void Update()
    {
        if (playerInRange && !hasTalked && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PlayDialog());
        }

        if (isPlayingDialog && Input.GetKeyDown(KeyCode.Tab))
        {
            SkipToNextLine();
        }
    }

    IEnumerator PlayDialog()
    {
        hasTalked = true;
        dialogUI.SetActive(true);
        isPlayingDialog = true;
        currentLine = 0;

        ShowLine();

        while (currentLine < dialogLines.Length)
        {
            float elapsed = 0f;
            while (elapsed < delayBetweenLines)
            {
                if (!isPlayingDialog) yield break;
                elapsed += Time.deltaTime;
                yield return null;
            }

            SkipToNextLine();
        }

        EndDialog();
    }

    void SkipToNextLine()
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
        dialogImage.sprite = dialogLines[currentLine].image;
        dialogImage.gameObject.SetActive(dialogLines[currentLine].image != null);
    }

    void EndDialog()
    {
        dialogUI.SetActive(false);
        isPlayingDialog = false;

        // 🎁 Rơi mảnh bản đồ
        if (mapPiecePrefab != null)
        {
            Vector2 randomOffset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Vector3 spawnPos = transform.position + (Vector3)randomOffset;

            GameObject mapPiece = Instantiate(mapPiecePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = mapPiece.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 dir = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rb.AddForce(dir * dropForce, ForceMode2D.Impulse);
            }
        }

        // 🧍‍♂️ Ẩn NPC nếu cần
        if (npcToHide != null)
        {
            npcToHide.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
