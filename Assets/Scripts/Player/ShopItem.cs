using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public enum StatType { Attack1, Defense1, Health1, Attack2, Defense2, Health2 }

    public StatType bonusType;
    public int bonusAmount = 100;
    public Button  buyButton;

    private PlayerHealth playerHealth;
    private PlayerCombat PlayerCombat;


    void Start()
    {
      //  Debug.Log("ShopItem Start gọi");

        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        PlayerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyItem);
          //  Debug.Log($"mau {playerHealth.currentHealth}");
            //PlayerCombat.AddAttack(50);

        }
        else
        {
         //   Debug.LogError("Buy Button chưa được gán!");
        }
        //Debug.Log("ShopItem Start gọi");
        //playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        //PlayerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();
        //buyButton.onClick.AddListener(BuyItem);
    }

    void BuyItem()
    {
       // Debug.Log("Đã bấm nút mua!");

        if (playerHealth == null || PlayerCombat == null)
        {
          //  Debug.LogError("Không tìm thấy Player!");
            return;
        }
        switch (bonusType)
        {
            case StatType.Attack1:
                PlayerCombat.AddAttack(1);
                break;
            case StatType.Defense1:
                playerHealth.Healing(1);
                break;
            case StatType.Health1:
                playerHealth.AddHealth(1);
                break;
            case StatType.Attack2:
                PlayerCombat.AddAttack(2);
                break;
            case StatType.Defense2:
                playerHealth.Healing(2);
                break;
            case StatType.Health2:
                playerHealth.AddHealth(2);
                break;
        }

        // Tuỳ chọn: Vô hiệu hoá sau khi mua
        buyButton.interactable = false;
    }
}
