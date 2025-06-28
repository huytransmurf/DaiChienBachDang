using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemy
{
    internal class BossKieuHealth : BossHealth
    {
        public override void TakeDamage(int damage)
        {
            BossKieu controller = GetComponent<BossKieu>();
            if (controller != null && controller.IsDead())
                return;
            animator.SetTrigger("Hurt");
            currentHealth -= damage;
            healthBar.SetHeal(currentHealth);

            if (currentHealth <= 0)
            {
                controller.HandleDeath();
                Die();
                GameManager.Instance.bossDefeated = true;
                if (keyPrefab != null)
                    DropKey();
                DropGold();
            }
            else
            {
                // Play hurt animation...
            }
        }
    }
}
