using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI.Quiz
{
    public class PaperClueTrigger : MonoBehaviour
    {
        private bool used = false;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!used && other.CompareTag("Player"))
            {
                used = true;
                QuizUIManager.Instance.ShowClue();
                Destroy(gameObject);
            }
        }
    }
}
