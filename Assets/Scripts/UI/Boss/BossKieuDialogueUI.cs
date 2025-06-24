using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI.Boss
{
    internal class BossKieuDialogueUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject dialoguePanel;
        public TMPro.TextMeshProUGUI dialogueText;
        public float displayTime = 2f;

        private Coroutine displayCoroutine;

        public void ShowDialogue(string text, float duration = 0f)
        {
            if (duration <= 0f)
                duration = displayTime;

            if (displayCoroutine != null)
                StopCoroutine(displayCoroutine);

            displayCoroutine = StartCoroutine(DisplayDialogueCoroutine(text, duration));
        }

        private IEnumerator DisplayDialogueCoroutine(string text, float duration)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = text;

            yield return new WaitForSeconds(duration);

            dialoguePanel.SetActive(false);
        }
    }
}
