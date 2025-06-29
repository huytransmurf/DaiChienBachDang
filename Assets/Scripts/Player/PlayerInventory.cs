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
    
    public void AddGold(int amount)
    {
        goldAmount += amount;
        GameManager.Instance.playerData.gold += amount;
        // Debug.Log("Vàng hiện có: " + goldAmount);
        UpdateGoldUI();

    }
    private void Start()
    {
        UpdateGoldUI();
        UpdateMapUI();
        if (specialMapButton != null)
            specialMapButton.gameObject.SetActive(false);
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
