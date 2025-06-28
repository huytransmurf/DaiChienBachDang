using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Quiz
{
    public class QuizUIManager : MonoBehaviour
    {
        public static QuizUIManager Instance;

        public GameObject cluePanel;
        public TMP_Text clueText;
        public Button nextButton;

        public GameObject answerPanel;
        public TMP_InputField answerInput;
        public Button submitButton;
        public TMP_Text resultText;

        private List<string> clues = new List<string>();
        private int clueIndex = 0;
        private string finalAnswer;

        void Awake()
        {
            Instance = this;
            cluePanel.SetActive(false);
            answerPanel.SetActive(false);

            nextButton.onClick.AddListener(HideClue);
            submitButton.onClick.AddListener(CheckAnswer);
        }

        public void SetData(string[] newClues, string answer)
        {
            clues = new List<string>(newClues);
            finalAnswer = answer;
            clueIndex = 0;
        }

        public void ShowClue()
        {
            if (clueIndex < clues.Count)
            {
                clueText.text = $"📜 Gợi ý {clueIndex + 1}: {clues[clueIndex]}";
                cluePanel.SetActive(true);
                clueIndex++;

                if (clueIndex == clues.Count)
                {
                    Invoke(nameof(ShowAnswerPanel), 1.5f);
                }
            }
        }

        void HideClue()
        {
            cluePanel.SetActive(false);
        }

        void ShowAnswerPanel()
        {
            answerPanel.SetActive(true);
        }

        void CheckAnswer()
        {
            string userInput = answerInput.text.Trim();
            if (string.Equals(userInput, finalAnswer, StringComparison.OrdinalIgnoreCase))
            {
                resultText.text = "✅ Chính xác!";
            }
            else
            {
                resultText.text = "❌ Sai rồi. Đáp án đúng là: " + finalAnswer;
            }
        }
    }
}
