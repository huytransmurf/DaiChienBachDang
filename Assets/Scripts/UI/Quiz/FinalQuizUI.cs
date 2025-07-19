using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Assets.Scripts.UI.Quiz;
using Assets.Scripts.Player;

public class FinalQuizUI : MonoBehaviour
{
    public static FinalQuizUI Instance;

    public GameObject panel;
    public TMP_Text[] quoteTexts; // 3 ô text
    public TMP_InputField answerInput;
    public Button confirmButton;
    public TMP_Text resultText;
    public GameObject cancelButton;

    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;
    private PlayerInventory playerGold;

    private List<string> quotes = new List<string>();
    private string correctAnswer;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        confirmButton.onClick.AddListener(CheckAnswer);
    }

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerCombat = player.GetComponent<PlayerCombat>();
        playerGold = player.GetComponent<PlayerInventory>(); // Đảm bảo bạn đã có script PlayerGold gắn vào Player
    }

    public void ShowFinalUI()
    {
        if (QuizLoader.QuizList.Count > 0)
        {
            quotes = QuizLoader.QuizList;
        }
        correctAnswer = QuizLoader.answerQuiz;

        panel.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < quoteTexts.Length && i < quotes.Count; i++)
        {
            quoteTexts[i].text = $"{quotes[i]}";
        }

        answerInput.text = "";
        resultText.text = "";
        confirmButton.interactable = true;
        cancelButton.SetActive(true);
    }

    public void CheckAnswer()
    {
        string userAnswer = answerInput.text.Trim().ToLower();

        if (string.Equals(userAnswer, correctAnswer.ToLower(), System.StringComparison.OrdinalIgnoreCase))
        {
            resultText.text = "Chính xác! Bạn nhận được 100 vàng và tăng sức mạnh";
            confirmButton.interactable = false;
            cancelButton.SetActive(false);

            // Thưởng người chơi
            playerCombat.AddAttack(10);
            playerHealth.AddHealth(50);
            playerGold.AddGold(100); // Thêm 100 vàng

            CoroutineRunner.Instance.StartCoroutine(CloseAfterDelay());
        }
        else
        {
            resultText.text = $"❌ Sai rồi. Hãy thử lại!";
            // Không đóng panel, người chơi tiếp tục trả lời
        }
    }

    private System.Collections.IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f); // Không bị ảnh hưởng bởi Time.timeScale
        ClosePanel();
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }
}
