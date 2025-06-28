using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils
{
    public class GPTQuizFetcher : MonoBehaviour
    {
        [Header("Your OpenAI API Key")]
        public string apiKey = "sk-xxxxxxxxxxxxxxxxxxxxx";

        [Header("Answer you want to ask GPT about")]
        public string finalAnswer = "Ngô Quyền";

        public IEnumerator FetchQuiz(Action<string[], string> onDone)
        {
            string prompt =
                $"Tạo 3 gợi ý lịch sử Việt Nam dẫn đến đáp án '{finalAnswer}'. Mỗi gợi ý < 20 từ. Trả JSON array.";

            string endpoint = "https://api.openai.com/v1/chat/completions";
            var bodyObj = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                temperature = 0.7
            };

            string jsonBody = JsonUtility
                .ToJson(bodyObj)
                .Replace("\\\"role\\\":null", "\"role\":\"user\"");
            jsonBody = jsonBody.Replace("\\\"content\\\":null", $"\"content\":\"{prompt}\"");

            UnityWebRequest req = new UnityWebRequest(endpoint, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var resultText = req.downloadHandler.text;
                string content = ExtractContentFromJson(resultText);
                string[] clues = JsonHelper.GetJsonArray(content);
                onDone.Invoke(clues, finalAnswer);
            }
            else
            {
                Debug.LogError("GPT API Error: " + req.error);
            }
        }

        private string ExtractContentFromJson(string json)
        {
            int index = json.IndexOf("\"content\":");
            int start = json.IndexOf('"', index + 10) + 1;
            int end = json.IndexOf("}", start);
            return json.Substring(start, end - start);
        }
    }
}
