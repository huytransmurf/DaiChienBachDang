using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWood : MonoBehaviour
{
    public static int woodCount = 0; // Số gỗ toàn cục

    public TextMeshProUGUI woodText; // Gán từ Canvas vào đây trong Unity
    [Header("Giao diện chính")]         // Hiển thị số gỗ
    public GameObject dialogUI;               // UI hộp thoại
    public TextMeshProUGUI dialogText;        // Nội dung hộp thoại
    public Image dialogImage;

    public float dialogDuration = 3f;
    private void Start()
    {
        woodCount = 0;
        UpdateWoodUI();
        dialogUI.SetActive(false);
    }

    public void AddWood(int amount)
    {
        if (woodCount >= 20) return; // Không thêm quá giới hạn

        woodCount += amount;
        woodCount = Mathf.Min(woodCount, 20); // Giới hạn tối đa là 20
        UpdateWoodUI();

        if (woodCount >= 20)
        {
            ShowDialog("Chúc mừng! Bạn đã nhặt đủ 20 khúc gỗ. Giờ hãy quay lại dân làng để nhận phần thưởng");
        }
    }

    void UpdateWoodUI()
    {
        if (woodText != null)
        {
            if (woodCount >= 20)
                woodText.text = "Đã nhặt đủ 20 khúc gỗ!";
            else
                woodText.text = woodCount + "/20";
        }
    }
    void ShowDialog(string message)
    {
        if (dialogUI != null && dialogText != null && dialogImage != null)
        {
            dialogUI.SetActive(true);
            dialogText.text = message;

            StartCoroutine(HideDialogAfterDelay(dialogDuration));
        }
    }

    IEnumerator HideDialogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogUI.SetActive(false);
    }
}
