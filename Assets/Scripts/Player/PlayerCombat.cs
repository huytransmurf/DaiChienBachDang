using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] public int baseDamage = 20;
        [SerializeField] public float baseCooldown = 1f;

        [Header("Attack Settings")]
        [SerializeField] public float attackRange = 1.5f;
        public Transform attackPoint;
        public LayerMask enemyLayer;

        // Stores cooldown duration for each skill by name
        private Dictionary<string, float> cooldownTimes;
        private Dictionary<string, float> skillRanges;
        private Dictionary<string, bool> lockedSkills;

        // Tracks the last time each skill was used
        private Dictionary<string, float> lastUsedTimes;

        public GameObject damagePopupPrefab; // gán trong Inspector
        public Canvas uiCanvas; // tham chiếu tới canvas chính

        void Start()
        {
            // Initialize cooldown durations
            cooldownTimes = new Dictionary<string, float>
            {
                { "NormalAttack", baseCooldown * 0f },
                { "Skill1", baseCooldown * 0f },
                { "Skill2", baseCooldown * 2f },
                { "Ultimate", baseCooldown * 6f }
            };

            skillRanges = new Dictionary<string, float>
            {
                { "NormalAttack", 1f },
                { "Skill1", 1.2f },
                { "Skill2", 1.8f },
                { "Ultimate", 2f }
            };

            lockedSkills = new Dictionary<string, bool>
            {
                { "NormalAttack", false },
                { "Skill1", true },
                { "Skill2", true },
                { "Ultimate", true }
            };

            // Set all skills to be ready at the start
            lastUsedTimes = new Dictionary<string, float>();
            foreach (var key in cooldownTimes.Keys)
            {
                lastUsedTimes[key] = -Mathf.Infinity;
            }
        }
        public void AddAttack(int amount)
        {
            int strengthamount = 0;

            if (amount == 1)
            {
                strengthamount = (int)(baseDamage * 0.2f); // Hồi 20% máu
            }
            else if (amount == 2)
            {
                strengthamount = (int)(baseDamage * 0.35f); // Hồi 35% máu
            }

            baseDamage += strengthamount;
            // Debug.Log("Tăng attack: " + baseDamage);
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
                DoDamage((int)(baseDamage * 0.5f), skillRanges["Skill1"]);
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
                    ShowDamagePopup(enemy.transform.position, damage);
                }
            }
        }
        private void ShowDamagePopup(Vector3 worldPosition, int damage)
        {
            // Chuyển vị trí thế giới sang màn hình
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition + Vector3.up * 1f);

            // Thêm offset ngẫu nhiên để tránh chồng popup
            float offsetX = Random.Range(-30f, 30f);
            float offsetY = Random.Range(-10f, 30f);
            screenPos += new Vector3(offsetX, offsetY, 0f);

            // Tạo và hiển thị popup
            GameObject popup = Instantiate(damagePopupPrefab, screenPos, Quaternion.identity, uiCanvas.transform);
            popup.GetComponent<DamagePopup>().SetDamage(damage);
        }


        /// <summary>
        /// Checks if a skill is ready to be used based on cooldown.
        /// </summary>
        public bool IsSkillReady(string skillName)
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
        public float GetSkillCooldownDuration(string skillName)
        {
            return cooldownTimes.ContainsKey(skillName) ? cooldownTimes[skillName] : 0f;
        }

        /// <summary>
        /// Check if a skill is currently locked.
        /// </summary>
        public bool IsSkillLocked(string skillName)
        {
            return lockedSkills.ContainsKey(skillName) && lockedSkills[skillName];
        }

        public void LockSkill(string skillName)
        {
            if (lockedSkills.ContainsKey(skillName))
                lockedSkills[skillName] = true;
        }

        public void UnlockSkill(string skillName)
        {
            if (lockedSkills.ContainsKey(skillName))
                lockedSkills[skillName] = false;
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
