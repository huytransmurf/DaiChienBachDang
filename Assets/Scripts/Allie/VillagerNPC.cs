using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VillagerDialogNPC : MonoBehaviour
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

    [Header("Map Piece Reward")]
    public GameObject mapPiecePrefab;
    public Transform dropPoint;
    public float dropForce = 3f;

    private bool hasTalked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PlayerWood.woodCount >= 20 && !hasTalked)
        {
            StartCoroutine(PlayDialog());
        }
    }

    IEnumerator PlayDialog()
    {
        hasTalked = true;
        dialogUI.SetActive(true);

        foreach (DialogLine line in dialogLines)
        {
            dialogText.text = line.text;
            dialogImage.sprite = line.image;

            yield return new WaitForSeconds(delayBetweenLines);
        }

        dialogUI.SetActive(false);
        DropMapPiece();
    }

    void DropMapPiece()
    {
        if (mapPiecePrefab != null && dropPoint != null)
        {
            // Tạo vị trí rơi ngẫu nhiên trong bán kính
            Vector2 randomOffset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Vector3 spawnPosition = dropPoint.position + (Vector3)randomOffset;

            GameObject mapPiece = Instantiate(mapPiecePrefab, spawnPosition, Quaternion.identity);

            // Thêm hiệu ứng rơi giống coin
            Rigidbody2D rb = mapPiece.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
            }
        }
    }

}
