using TMPro;
using UnityEngine;

public class PlayerWood : MonoBehaviour
{
    public static int woodCount = 0; // Số gỗ toàn cục

    public TextMeshProUGUI woodText; // Gán từ Canvas vào đây trong Unity

    private void Start()
    {
        UpdateWoodUI();
    }

    public void AddWood(int amount)
    {
        woodCount += amount;
        UpdateWoodUI();
    }

    void UpdateWoodUI()
    {
        if (woodText != null)
        {
            woodText.text = "Số gỗ cần nhặt: " + woodCount + "/20";
        }
    }
}
