using Assets.Scripts.UI.HealBar;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    public bool hasKey = false;
    public int goldAmount = 0;
    public TextMeshProUGUI goldText;
    public int potionCount = 0;
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
}
