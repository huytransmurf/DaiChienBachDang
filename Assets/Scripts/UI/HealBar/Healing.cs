using Assets.Scripts.Player;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.HealBar
{
    public class Healing : MonoBehaviour
    {
        public int healAmount = 20;
        public KeyCode healKey = KeyCode.E;

        private PlayerHealth playerHealth;
        private PlayerInventory inventory;

        public TextMeshProUGUI healCountText;
        public static Healing instance;
        void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<PlayerHealth>();
                inventory = playerObj.GetComponent<PlayerInventory>();
            }

            UpdateHealUI();
        }

        void Update()
        {
            UpdateHealUI();
            if (Input.GetKeyDown(healKey) && inventory != null && inventory.potionCount > 0)
            {
                HealPlayer();
            }
        }

        void HealPlayer()
        {
            if (playerHealth == null || inventory == null)
                return;

            playerHealth.HealHealth(healAmount);
            inventory.potionCount--;
            //Debug.Log("Đã dùng bình máu, còn lại: " + inventory.potionCount);
            UpdateHealUI();
        }

        public void UpdateHealUI()
        {
            if (healCountText != null && inventory != null)
            {
                healCountText.text = $"{inventory.potionCount}";
            }
        }
    }
}
