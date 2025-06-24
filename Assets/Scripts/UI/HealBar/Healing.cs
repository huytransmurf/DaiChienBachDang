using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.HealBar
{
    public class Healing : MonoBehaviour
    {
        public int maxHeals = 5;
        public int healAmount = 20;
        public KeyCode healKey = KeyCode.L;

        private int currentHeals;
        private PlayerHealth playerHealth;

        public TextMeshProUGUI healCountText;

        void Start()
        {
            if (healCountText != null)
                healCountText.text = string.Format("{0}", maxHeals);
            currentHeals = maxHeals;
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }

            UpdateHealUI();
        }

        void Update()
        {
            if (Input.GetKeyDown(healKey) && currentHeals > 0)
            {
                HealPlayer();
            }
        }

        void HealPlayer()
        {
            if (playerHealth == null)
                return;

            playerHealth.HealHealth(healAmount);
            currentHeals--;

            Debug.Log("Healed! Remaining heals: " + currentHeals);
            UpdateHealUI();
        }

        void UpdateHealUI()
        {
            if (healCountText != null)
                healCountText.text = string.Format("{0}", currentHeals);
        }
    }
}
