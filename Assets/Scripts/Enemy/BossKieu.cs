using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Player;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class BossKieu : BaseBossController
    {
        [Header("Attack Effects")]
        public GameObject laserPrefab; // Chiêu 3
        public Transform laserSpawnPoint;
        public GameObject comboEffectPrefab; // Chiêu 2
        public Transform comboEffectPoint;

        protected override void OnAttack1()
        {
            // Chiêu 1: Đánh thường
            // Tạm thời không cần hiệu ứng đặc biệt, chỉ play animation và deal damage gần
            Debug.Log("Boss uses basic melee attack");
        }

        protected override void OnAttack2()
        {
            Debug.Log("Boss uses combo attack 2");
            // Chiêu 2: Combo 2 đòn diện rộng
            StartCoroutine(ComboAOE());
        }

        protected override void OnAttack3()
        {
            // Chiêu 3: Bắn laser
            if (laserPrefab != null && laserSpawnPoint != null)
            {
                GameObject laser = Instantiate(
                    laserPrefab,
                    laserSpawnPoint.position,
                    laserSpawnPoint.rotation
                );
                Debug.Log("Boss fires a laser beam!");
            }
        }

        private IEnumerator ComboAOE()
        {
            if (comboEffectPrefab != null && comboEffectPoint != null)
            {
                // Phát 1
                Instantiate(comboEffectPrefab, comboEffectPoint.position, Quaternion.identity);
                DealAOEDamage(stats.attack2Damage, stats.attack2Range);
                yield return new WaitForSeconds(0.4f);

                // Phát 2
                Instantiate(
                    comboEffectPrefab,
                    comboEffectPoint.position + new Vector3(1f, 0, 0),
                    Quaternion.identity
                );
                DealAOEDamage(stats.attack2Damage, stats.attack2Range);
            }
        }

        private void DealAOEDamage(float damage, float range)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, playerLayer);
            foreach (var hit in hits)
            {
                PlayerHealth target = hit.GetComponent<PlayerHealth>();
                if (target != null)
                {
                    target.TakeDamage((int)damage);
                }
            }
        }
    }
}
