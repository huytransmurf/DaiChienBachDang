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

    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    public GameObject cancelButton;

    private List<string> quotes = new List<string>();
    private string correctAnswer;
    private void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        playerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();
    }

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        confirmButton.onClick.AddListener(CheckAnswer);
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
    }

    public void CheckAnswer()
    {
        string userAnswer = answerInput.text.Trim();

        if (string.Equals(userAnswer, correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            resultText.text = "✅ Chính xác!";
        }
        else
        {
            resultText.text = $"❌ Sai rồi. Đáp án là: {correctAnswer}";
        }

        CoroutineRunner.Instance.StartCoroutine(CloseAfterDelay());
        playerCombat.AddAttack(10);
        playerHealth.AddHealth(50);
        
        cancelButton.SetActive(false);
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
