using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Base Stats")]
        public int baseDamage = 20;
        public float baseCooldown = 1f;

        [Header("Attack Settings")]
        public float attackRange = 1.5f;
        public Transform attackPoint;
        public LayerMask enemyLayer;

        private Dictionary<string, SkillData> skillDict;
        private Dictionary<string, float> lastUsedTimes;

        public GameObject damagePopupPrefab;
        public Canvas uiCanvas;

        void Start()
        {
            skillDict = new Dictionary<string, SkillData>
            {
                {
                    "NormalAttack",
                    new SkillData
                    {
                        skillName = "NormalAttack",
                        baseDamageMultiplier = 1f,
                        baseRange = 1f,
                        cooldown = 0f,
                        isLocked = false
                    }
                },
                {
                    "Skill1",
                    new SkillData
                    {
                        skillName = "Skill1",
                        baseDamageMultiplier = 0.5f,
                        baseRange = 1.2f,
                        cooldown = 0f
                    }
                },
                {
                    "Skill2",
                    new SkillData
                    {
                        skillName = "Skill2",
                        baseDamageMultiplier = 3f,
                        baseRange = 1.8f,
                        cooldown = 2f
                    }
                },
                {
                    "Ultimate",
                    new SkillData
                    {
                        skillName = "Ultimate",
                        baseDamageMultiplier = 5f,
                        baseRange = 2f,
                        cooldown = 6f
                    }
                }
            };

            var data = GameManager.Instance.playerData;

            foreach (var kv in skillDict)
            {
                string name = kv.Key;

                if (data.unlockedSkills.ContainsKey(name) && data.unlockedSkills[name])
                    kv.Value.Unlock();

                if (data.upgradedSkills.ContainsKey(name))
                    kv.Value.level = data.upgradedSkills[name];
            }

            lastUsedTimes = new Dictionary<string, float>();
            foreach (var skill in skillDict.Keys)
                lastUsedTimes[skill] = -Mathf.Infinity;
        }

        // ========================
        // ATTACK METHODS
        // ========================
        public void NormalAttack() => UseSkill("NormalAttack");

        public void Skill1() => UseSkill("Skill1");

        public void Skill2() => UseSkill("Skill2");

        public void Ultimate() => UseSkill("Ultimate");

        private void UseSkill(string skillName)
        {
            if (!IsSkillReady(skillName))
                return;

            var skill = skillDict[skillName];
            lastUsedTimes[skillName] = Time.time;

            float finalDamage = baseDamage * skill.GetCurrentDamageMultiplier();
            float finalRange = skill.GetCurrentRange();
            DoDamage((int)finalDamage, finalRange);
        }

        // ========================
        // DAMAGE
        // ========================
        private void DoDamage(int damage, float range)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                range,
                enemyLayer
            );
            foreach (var enemy in enemies)
            {
                Debug.Log("Hit " + enemy.name);
                if (enemy.GetComponent<BossDeKieuHealth>() != null)
                {
                    var health = enemy.GetComponent<BossDeKieuHealth>();
                    if (health != null)
                    {
                        health.TakeDamage(damage);
                        ShowDamagePopup(enemy.transform.position, damage);
                    }
                }
                else if (enemy.GetComponent<BossHealth>() != null)
                {
                    var bossHealth = enemy.GetComponent<BossHealth>();
                    if (bossHealth != null)
                    {
                        bossHealth.TakeDamage(damage);
                        ShowDamagePopup(enemy.transform.position, damage);
                    }
                }
                else if (enemy.GetComponent<BossKieuHealth>() != null)
                {
                    var health = enemy.GetComponent<BossKieuHealth>();
                    if (health != null)
                    {
                        health.TakeDamage(damage);
                        ShowDamagePopup(enemy.transform.position, damage);
                    }
                }
                else if (enemy.GetComponent<Enemy.EnemyHealth>() != null)
                {
                    var health = enemy.GetComponent<Enemy.EnemyHealth>();
                    if (health != null)
                    {
                        health.TakeDamage(damage);
                        ShowDamagePopup(enemy.transform.position, damage);
                    }
                }
            }
        }

        public void AddAttack(int amount)
        {
            int strengthamount = 0;

            if (amount == 1)
            {
                strengthamount = (int)(baseDamage * 0.2f);
            }
            else if (amount == 2)
            {
                strengthamount = (int)(baseDamage * 0.35f);
            }
            baseDamage += strengthamount; // ✅ Dòng này mới thực sự tăng baseDamage
            Debug.Log($"Tăng baseDamage: +{strengthamount} => {baseDamage}");
        }

        private void ShowDamagePopup(Vector3 worldPos, int damage)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + Vector3.up);
            screenPos += new Vector3(Random.Range(-30f, 30f), Random.Range(-10f, 30f));
            GameObject popup = Instantiate(
                damagePopupPrefab,
                screenPos,
                Quaternion.identity,
                uiCanvas.transform
            );
            popup.GetComponent<DamagePopup>().SetDamage(damage);
        }

        // ========================
        // COOLDOWN & UPGRADE
        // ========================
        public bool IsSkillReady(string skillName)
        {
            if (!skillDict.ContainsKey(skillName))
                return false;
            float lastTime = lastUsedTimes[skillName];
            return Time.time >= lastTime + skillDict[skillName].cooldown;
        }

        public float GetRemainingCooldown(string skillName)
        {
            if (!skillDict.ContainsKey(skillName))
                return 0f;

            float lastTime = lastUsedTimes.ContainsKey(skillName)
                ? lastUsedTimes[skillName]
                : -Mathf.Infinity;
            float cooldown = skillDict[skillName].cooldown;

            return Mathf.Max(0f, cooldown - (Time.time - lastTime));
        }

        public float GetSkillCooldownDuration(string skillName)
        {
            return skillDict.ContainsKey(skillName) ? skillDict[skillName].cooldown : 0f;
        }

        public void UnlockSkill(string skillName)
        {
            if (skillDict.ContainsKey(skillName))
            {
                skillDict[skillName].Unlock();
                GameManager.Instance.playerData.unlockedSkills[skillName] = true;
            }
        }

        public void UpgradeSkill(string skillName)
        {
            if (skillDict.ContainsKey(skillName))
            {
                skillDict[skillName].Upgrade();
                GameManager.Instance.playerData.upgradedSkills[skillName] = skillDict[
                    skillName
                ].level;
            }
        }

        public bool IsSkillLocked(string skillName)
        {
            if (skillDict.ContainsKey(skillName))
            {
                return skillDict[skillName].isLocked;
            }
            return true;
        }

        public bool IsSkillUpgraded(string skillName)
        {
            return skillDict.ContainsKey(skillName) && skillDict[skillName].level > 0;
        }

        void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public float baseDamageMultiplier;
        public float baseRange;
        public float cooldown;
        public bool isLocked = true;
        public int level = 0;

        public float GetCurrentDamageMultiplier() => baseDamageMultiplier * (1f + 0.2f * level);

        public float GetCurrentRange() => baseRange * (1f + 0.2f * level);

        public bool IsUnlocked() => !isLocked;

        public void Unlock() => isLocked = false;

        public void Upgrade() => level++;
    }
}
