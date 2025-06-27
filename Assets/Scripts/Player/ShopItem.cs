using Assets.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public enum StatType { Attack1, Defense1, Health1, Attack2, Defense2, Health2, Skill1, Skill2, Ultimate }

    public StatType bonusType;
    public Button  buyButton;

    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    public int bonusAmount;
    public TextMeshProUGUI dialogText;

    private float messageDuration = 2f;
    public GameObject dialogBox;


    void Start()
    {
        //  Debug.Log("ShopItem Start gọi");
        if (dialogBox != null)
            dialogBox.SetActive(false);
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        playerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyItem);
          //  Debug.Log($"mau {playerHealth.currentHealth}");
            //playerCombat.AddAttack(50);

        }
        else
        {
         //   Debug.LogError("Buy Button chưa được gán!");
        }
        //Debug.Log("ShopItem Start gọi");
        //playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        //playerCombat = GameObject.FindWithTag("Player").GetComponent<playerCombat>();
        //buyButton.onClick.AddListener(BuyItem);
    }

    void BuyItem()
    {
       // Debug.Log("Đã bấm nút mua!");

        if (playerHealth == null || playerCombat == null)
        {
          //  Debug.LogError("Không tìm thấy Player!");
            return;
        }



        if (PlayerInventory.instance.goldAmount < bonusAmount)
        {
            ShowDialog("Không đủ vàng để mua! Cần " + bonusAmount + " vàng.");
            return;
        }

        PlayerInventory.instance.AddGold(-bonusAmount);
        
        switch (bonusType)
        {
            case StatType.Attack1:
                playerCombat.AddAttack(1);
                ShowDialog(" Đã mua thành công! Strength hiện tại: " + playerCombat.baseDamage);
                break;

            case StatType.Defense1:
                playerHealth.Healing(1);
                ShowDialog(" Đã mua thành công! Heal hiện tại: " + playerHealth.currentHealth);
                break;

            case StatType.Health1:
                playerHealth.AddHealth(1);
                ShowDialog(" Đã mua thành công! Heal hiện tại: " + playerHealth.maxHealth);
                break;

            case StatType.Attack2:
                playerCombat.AddAttack(2);
                ShowDialog(" Đã mua thành công! Strength hiện tại: " + playerCombat.baseDamage);
                break;

            case StatType.Defense2:
                playerHealth.Healing(2);
                ShowDialog(" Đã mua thành công! Heal hiện tại: " + playerHealth.currentHealth);
                break;

            case StatType.Health2:
                playerHealth.AddHealth(2);
                ShowDialog(" Đã mua thành công! Heal hiện tại: " + playerHealth.maxHealth);
                break;

            case StatType.Skill1:
                playerCombat.UnlockSkill("Skill1");
                Debug.Log("Đã mở khóa Skill1");
                break;

            case StatType.Skill2:
                playerCombat.UnlockSkill("Skill2");
                Debug.Log("Đã mở khóa Skill2");
                break;

            case StatType.Ultimate:
                playerCombat.UnlockSkill("Ultimate");
                Debug.Log("Đã mở khóa Ultimate");
                break;
        }

        // Tuỳ chọn: Vô hiệu hoá sau khi mua
        buyButton.interactable = false;
    }
    void ShowDialog(string message)
    {
        if (dialogBox != null && dialogText != null)
        {
            dialogText.text = message;
            dialogBox.SetActive(true);
            CancelInvoke(nameof(HideDialog));
            Invoke(nameof(HideDialog), messageDuration);
        }
    }
    void HideDialog()
    {
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
    }
}
