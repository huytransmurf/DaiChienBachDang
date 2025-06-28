using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils
{
    public class GPTQuizFetcher : MonoBehaviour
    {
        [Header("Your Gemini API Key")]
        [TextArea]
        public string apiKey = "AIzaSyCrbceVHfLc8ks2v9tu6W90sIi6jxtcvcg";

        [Header("Final Answer")]
        public string finalAnswer = "Ngô Quyền";

        string[] famousVietnameseFigures = new string[]
        {
            // Cổ - Trung đại
            "Thánh Gióng",
            "An Dương Vương",
            "Bà Triệu",
            "Hai Bà Trưng",
            "Ngô Quyền",
            "Đinh Bộ Lĩnh",
            "Lê Hoàn",
            "Lý Thái Tổ",
            "Lý Thường Kiệt",
            "Trần Thủ Độ",
            "Trần Hưng Đạo",
            "Trần Nhân Tông",
            "Trần Quang Khải",
            "Trần Khánh Dư",
            "Trần Quốc Toản",
            "Lê Lợi",
            "Nguyễn Trãi",
            "Lê Thánh Tông",
            "Lê Lai",
            "Nguyễn Bỉnh Khiêm",
            "Nguyễn Huệ",
            "Ngô Thì Nhậm",
            "Phan Huy Ích",
            "Chu Văn An",
            "Mạc Đĩnh Chi",
            "Đoàn Thị Điểm",
            "Bùi Thị Xuân",
            // Chống Pháp - Nhật
            "Cao Bá Quát",
            "Phan Đình Phùng",
            "Trương Định",
            "Hoàng Hoa Thám",
            "Nguyễn Lộ Trạch",
            "Nguyễn Tri Phương",
            "Nguyễn Công Trứ",
            "Nguyễn Khuyến",
            "Nguyễn Đình Chiểu",
            "Phan Bội Châu",
            "Phan Chu Trinh",
            "Lương Văn Can",
            "Đặng Thái Thân",
            "Đặng Thúc Hứa",
            "Nguyễn Thái Học",
            "Nguyễn An Ninh",
            "Nguyễn Văn Cừ",
            // Hiện đại
            "Hồ Chí Minh",
            "Trường Chinh",
            "Phạm Văn Đồng",
            "Lê Duẩn",
            "Võ Nguyên Giáp",
            "Tôn Đức Thắng",
            "Lê Đức Thọ",
            "Nguyễn Thị Định",
            "Nguyễn Văn Trỗi",
            "Võ Thị Sáu",
            "Tô Hiệu",
            "Nguyễn Thị Minh Khai",
            "Trần Phú",
            "Nguyễn Chí Thanh",
            "Văn Tiến Dũng",
            "Phạm Hùng",
            "Dương Quang Hàm",
            "Nguyễn Sơn",
            "Nguyễn Hữu Thọ"
        };

        public IEnumerator FetchQuiz(Action<string[], string> onDone)
        {
            string selectedHero = famousVietnameseFigures[
                UnityEngine.Random.Range(0, famousVietnameseFigures.Length)
            ];
            string prompt =
                $"Tạo 3 gợi ý lịch sử Việt Nam dẫn đến đáp án '{selectedHero}'. Mỗi gợi ý < 20 từ. Trả về JSON array.";

            string endpoint =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var body = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };

            string safeJson = JsonConvert.SerializeObject(body);

            UnityWebRequest req = new UnityWebRequest(endpoint, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(safeJson);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("x-goog-api-key", apiKey);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string resultText = req.downloadHandler.text;
                Debug.Log("✅ Gemini raw response:\n" + resultText);

                string[] clues = ExtractContentArray(resultText);
                if (clues != null && clues.Length > 0)
                {
                    onDone?.Invoke(clues, selectedHero);
                }
                else
                {
                    Debug.LogWarning("⚠ Không tách được mảng gợi ý.");
                }
            }
            else
            {
                Debug.LogError("❌ Gemini API Error: " + req.error);
                Debug.LogError("📨 Response:\n" + req.downloadHandler.text);
            }
        }

        private string ExtractContentFromGeminiResponse(string json)
        {
            try
            {
                var parsed = JObject.Parse(json);
                string text = parsed["candidates"]
                    ?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                return text;
            }
            catch (Exception e)
            {
                Debug.LogWarning("⚠ Lỗi khi trích xuất nội dung từ Gemini: " + e.Message);
                return null;
            }
        }

        private string[] ExtractContentArray(string json)
        {
            try
            {
                var parsed = JObject.Parse(json);
                string rawText = parsed["candidates"]
                    ?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                if (string.IsNullOrEmpty(rawText))
                    return null;

                // 🔧 Bỏ code block markdown ```json ... ```
                if (rawText.StartsWith("```"))
                {
                    int start = rawText.IndexOf('\n') + 1;
                    int end = rawText.LastIndexOf("```");
                    rawText = rawText.Substring(start, end - start).Trim();
                }

                // ✅ Parse thành mảng string
                string[] clues = JsonConvert.DeserializeObject<string[]>(rawText);
                return clues;
            }
            catch (Exception e)
            {
                Debug.LogWarning("⚠ Lỗi khi tách array từ Gemini: " + e.Message);
                return null;
            }
        }
    }
}
