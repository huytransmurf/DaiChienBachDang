using Assets.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public string skillName;
    public Image cooldownImage;        // Cooldown overlay (filled image)
    public TextMeshProUGUI cooldownText;          // Countdown text
    public TextMeshProUGUI keyText;               // Key hint (ví dụ: "I" hoặc "J")
    public GameObject lockOverlay;     // Optional: icon or text showing skill is locked/unavailable

    public PlayerCombat playerCombat;

    void Start()
    {
        // Tự tìm PlayerCombat nếu chưa gán
        if (playerCombat == null)
        {
            playerCombat = GetComponent<PlayerCombat>();
        }
    }

    void Update()
    {
        if (playerCombat == null) return;

        if(playerCombat.IsSkillLocked(skillName))
        {
            lockOverlay.SetActive(true);
            cooldownImage.fillAmount = 100;
            cooldownText.text = "";
            keyText.text = ""; 
            return;
        }
        else
        {
            lockOverlay.SetActive(false);
            float remaining = playerCombat.GetRemainingCooldown(skillName);
            float max = playerCombat.GetSkillCooldownDuration(skillName);

            if (remaining > 0)
            {
                cooldownImage.fillAmount = remaining / max;

                if (remaining >= 1f)
                    cooldownText.text = Mathf.CeilToInt(remaining).ToString();
                else
                    cooldownText.text = remaining.ToString("F1");  // hiển thị 0.9, 0.4...
            }
            else
            {
                cooldownImage.fillAmount = 0;
                cooldownText.text = "";
            }
        }
    }
}
