
using System.Collections.Generic;
using Assets.Scripts.UI.Quiz;
using NUnit.Framework;
using TMPro;
using UnityEngine;



public class MapPiece : MonoBehaviour
{
    public static int index;

    private List<string> quotes = new List<string>
    {
        "Không có gì quý hơn độc lập tự do.",
        "Ta thà làm quỷ nước Nam còn hơn làm vương đất Bắc.",
        "Lấy đại nghĩa để thắng hung tàn, lấy chí nhân để thay cường bạo."
    };

     void Update()
    {
        if(QuizLoader.QuizList.Count > 0)
        quotes = QuizLoader.QuizList;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            string randomQuote = quotes[index];
            QuoteDisplayUI.Instance.ShowQuote(randomQuote);
            PlayerInventory.instance.AddMapPiece();

            Destroy(gameObject);
            index++;// hoặc ẩn nó đi
        }
    }

   
}

