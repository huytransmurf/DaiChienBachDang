using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemy
{
    internal class BossDeKieuHealth : BossHealth
    {
        public override void TakeDamage(int damage)
        {
            BossDeKieu controller = GetComponent<BossDeKieu>();
            if (controller != null && controller.IsDead)
                return;
            animator.SetTrigger("Hurt");
            currentHealth -= damage;
            healthBar.SetHeal(currentHealth);

            if (currentHealth <= 0)
            {
                controller.Die();
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
