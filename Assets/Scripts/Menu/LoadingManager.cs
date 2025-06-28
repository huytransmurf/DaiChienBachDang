using Assets.Scripts.Menu;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI tipText;

    [Header("Tips")]
    public string[] loadingTips = {
        "Mẹo: Luôn quan sát để tránh bị phục kích!",
        "Mẹo: Dùng kỹ năng né đúng lúc để không mất máu.",
        "Mẹo: Nhặt đồ hiếm trước khi đánh boss!",
        "Mẹo: Bấm [P] để mở cài đặt bất cứ lúc nào."
    };

    void Start()
    {
        ShowRandomTip();
        UpdateUI(0f);
        StartCoroutine(LoadSceneAsync());
    }

    void ShowRandomTip()
    {
        if (tipText != null && loadingTips.Length > 0)
        {
            int index = Random.Range(0, loadingTips.Length);
            tipText.text = loadingTips[index];
        }
    }

    IEnumerator LoadSceneAsync()
    {
        float displayProgress = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingData.targetScene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Làm mượt: chạy từ từ đến targetProgress
            displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, Time.deltaTime);

            UpdateUI(displayProgress);

            // Khi Unity load xong (đứng ở 0.9), ta cho chạy tiếp đến 1
            if (displayProgress >= 0.99f && operation.progress >= 0.9f)
            {
                UpdateUI(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }


    void UpdateUI(float progress)
    {
        if (progressBar != null)
            progressBar.value = progress;

        if (progressText != null)
            progressText.text = $"Đang tải... {Mathf.RoundToInt(progress * 100)}%";
    }
}