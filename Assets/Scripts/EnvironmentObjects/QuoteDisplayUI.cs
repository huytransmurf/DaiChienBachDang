using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuoteDisplayUI : MonoBehaviour
{
    public static QuoteDisplayUI Instance;

    public GameObject panel;
    public TMP_Text quoteText;
    public Button closeButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        closeButton.onClick.AddListener(HideQuote);
    }

    public void ShowQuote(string quote)
    {
        quoteText.text = $"{quote}";
        panel.SetActive(true);
        Time.timeScale = 0f; // Pause game nếu muốn
    }

    public void HideQuote()
    {
        panel.SetActive(false);
        Time.timeScale = 1f; // Resume game nếu dùng pause
    }
}
