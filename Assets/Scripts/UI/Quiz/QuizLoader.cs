using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.UI.Quiz
{
    public class QuizLoader : MonoBehaviour
    {
        private GPTQuizFetcher fetcher;
        public static List<string> QuizList = new List<string>();
        public static string answerQuiz;
        void Start()
        {
            fetcher = GetComponent<GPTQuizFetcher>();
            StartCoroutine(fetcher.FetchQuiz(OnQuizReady));
        }

        void OnQuizReady(string[] clues, string answer)
        {
            Debug.Log("GPT đã trả về dữ liệu:");
            foreach (string clue in clues)
            {
                Debug.Log("🧠 Clue: " + clue);
                QuizList.Add(clue);
            }
            Debug.Log("🎯 Final Answer: " + answer);
            answerQuiz = answer;    

            // Gửi qua UI manager
            //QuizUIManager.Instance.SetData(clues, answer);
        }
    }
}
