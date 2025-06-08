using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        public int attackDamage = 100;
        public float attackRange = 1f;
        public float attackCooldown = 0.5f;
        public Transform attackPoint;
        public LayerMask enemyLayer;

        private float lastAttackTime = 0f;


        void Update()
        {
            if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        void Attack()
        {
            // Optional: trigger attack animation here

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

            Debug.Log("hitEnemies.Length: " + hitEnemies.Length);

            foreach (Collider2D enemy in hitEnemies)
            {

                Enemy.EnemyHealth health = enemy.GetComponent<Enemy.EnemyHealth>();
                Debug.Log("health: " + health);
                if (health != null)
                {
                    Debug.Log("TakeDamage: " + enemy.name); 
                    health.TakeDamage(attackDamage);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
