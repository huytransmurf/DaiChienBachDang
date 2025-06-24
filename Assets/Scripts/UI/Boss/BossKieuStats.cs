using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI.Boss
{
    internal class BossKieuStats
    {
        public string[] dialogues =
        {
            "You cannot defeat me!",
            "Is that all you've got?",
            "Pathetic!",
            "I am unstoppable!",
            "You will regret challenging me!",
            "Feel my power!",
            "This is child's play!",
            "You're nothing but an insect!",
            "Bow before my might!",
            "I will crush you!"
        };

        [Header("Dialogue Audio")]
        public AudioClip[] dialogueSounds;

        [Header("Dialogue Settings")]
        public float dialogueInterval = 5f;
        public bool enableRandomDialogue = true;
    }
}
