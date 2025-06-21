using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Base Stats")]
        public int baseDamage = 20;
        public float baseCooldown = 0.5f;

        [Header("Attack Settings")]
        public float attackRange = 1.5f;
        public Transform attackPoint;
        public LayerMask enemyLayer;

        // Stores cooldown duration for each skill by name
        private Dictionary<string, float> cooldownTimes;
        private Dictionary<string, float> skillRanges;

        // Tracks the last time each skill was used
        private Dictionary<string, float> lastUsedTimes;

        void Start()
        {
            // Initialize cooldown durations
            cooldownTimes = new Dictionary<string, float>
            {
                { "NormalAttack", baseCooldown },
                { "Skill1", baseCooldown * 6f },
                { "Skill2", baseCooldown * 10f },
                { "Ultimate", baseCooldown * 15f }
            };

            skillRanges = new Dictionary<string, float>
            {
                { "NormalAttack", 1f },
                { "Skill1", 1.2f },
                { "Skill2", 1.8f },
                { "Ultimate", 2f }
            };

            // Set all skills to be ready at the start
            lastUsedTimes = new Dictionary<string, float>();
            foreach (var key in cooldownTimes.Keys)
            {
                lastUsedTimes[key] = -Mathf.Infinity;
            }
        }

        // =========================
        // Called from animation events
        // =========================

        /// <summary>
        /// Normal attack (basic skill).
        /// </summary>
        public void NormalAttack()
        {
            if (IsSkillReady("NormalAttack"))
            {
                lastUsedTimes["NormalAttack"] = Time.time;
                DoDamage(baseDamage, skillRanges["NormalAttack"]);
            }
        }

        /// <summary>
        /// Skill 1 - stronger than basic attack.
        /// </summary>
        public void Skill1()
        {
            if (IsSkillReady("Skill1"))
            {
                lastUsedTimes["Skill1"] = Time.time;
                DoDamage((int)(baseDamage * 1.5f), skillRanges["Skill1"]);
            }
        }

        /// <summary>
        /// Skill 2 - medium-power skill.
        /// </summary>
        public void Skill2()
        {
            if (IsSkillReady("Skill2"))
            {
                lastUsedTimes["Skill2"] = Time.time;
                DoDamage((int)(baseDamage * 3f), skillRanges["Skill2"]);
            }
        }

        /// <summary>
        /// Ultimate skill - strongest skill.
        /// </summary>
        public void Ultimate()
        {
            if (IsSkillReady("Ultimate"))
            {
                lastUsedTimes["Ultimate"] = Time.time;
                DoDamage((int)(baseDamage * 5f), skillRanges["Ultimate"]);
            }
        }

        // =========================
        // Damage Logic
        // =========================

        /// <summary>
        /// Apply damage to all enemies in range.
        /// </summary>
        void DoDamage(int damage, float range)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                range,
                enemyLayer
            );

            foreach (Collider2D enemy in hitEnemies)
            {
                var health = enemy.GetComponent<Enemy.EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    Debug.Log($"Hit {enemy.name} for {damage} damage.");
                }
            }
        }

        /// <summary>
        /// Checks if a skill is ready to be used based on cooldown.
        /// </summary>
        bool IsSkillReady(string skillName)
        {
            Debug.Log($"Checking cooldown for {skillName}");
            float lastTime = lastUsedTimes[skillName];
            float cooldown = cooldownTimes[skillName];
            return Time.time >= lastTime + cooldown;
        }

        /// <summary>
        /// Get remaining cooldown time for a specific skill.
        /// </summary>
        public float GetRemainingCooldown(string skillName)
        {
            float lastTime = lastUsedTimes[skillName];
            float cooldown = cooldownTimes[skillName];
            return Mathf.Max(0, cooldown - (Time.time - lastTime));
        }

        /// <summary>
        /// Draws the attack range in the editor.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
