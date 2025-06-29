using Assets.Scripts.UI.HealBar;
using Assets.Scripts.UI.Quiz;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    public bool hasKey = false;
    public int goldAmount = 0;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI mapText;

    public int potionCount = 0;
    [Header("Bản đồ")]
    public int mapPieceCount = 0;
    public int totalMapPieces = 3; // hoặc số mảnh bạn cần để hoàn thành bản đồ

    public Button specialMapButton;
    public void AddMapPiece()
    {
        mapPieceCount++;

        UpdateMapUI();
        //Debug.Log($"Đã nhặt mảnh bản đồ: {mapPieceCount}/{totalMapPieces}");
        if (HasAllMapPieces() && specialMapButton != null)
        {
            specialMapButton.gameObject.SetActive(true); // Hiện nút
        }

        goldAmount = GameManager.Instance.playerData.gold;

        UpdateGoldUI();
        UpdateMapUI();

    }

    public bool HasAllMapPieces()
    {
        return mapPieceCount >= totalMapPieces;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Gán vàng từ GameManager (chỉ làm nếu Instance và playerData không null)
        if (GameManager.Instance != null && GameManager.Instance.playerData != null)
        {
            goldAmount = GameManager.Instance.playerData.gold;
            Debug.Log($"Start - Load vàng từ GameManager: {goldAmount}");
        }

        UpdateGoldUI();
        UpdateMapUI();

        if (specialMapButton != null)
            specialMapButton.gameObject.SetActive(false);
    }

    public void AddGold(int amount)
    {
        goldAmount += amount;
        GameManager.Instance.playerData.gold += amount;
        // Debug.Log("Vàng hiện có: " + goldAmount);
        UpdateGoldUI();

    }

    public void AddPotion(int amount)
    {
        potionCount += amount;

        // Cập nhật UI nếu Healing đã khởi tạo
        if (Healing.instance != null)
        {
            Healing.instance.UpdateHealUI();
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"{goldAmount}";
            Debug.Log($"Cập nhật vàng: {goldAmount}");
        }
    }
    private void UpdateMapUI()
    {
        if (mapText != null)
        {
            mapText.text = $"{mapPieceCount}/{totalMapPieces}";
        }
    }
}
