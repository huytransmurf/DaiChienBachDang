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

        public override void OnAttack1()
        {
            Debug.Log("Boss uses basic melee attack");
        }

        protected override void OnAttack2()
        {
            Debug.Log("Boss uses combo attack 2");
            //StartCoroutine(ComboAOE());
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

        protected override IEnumerator PerformAttack3()
        {
            FacePlayer();
            PlaySound(GetAttackSound());

            yield return new WaitForSeconds(0.6f);
            DealDamageAttack3(20, new Vector2(4f, 1f), Vector2.right * 1.5f);
            OnAttack3();

            yield return new WaitForSeconds(0.7f);

            canChangeState = true;
            MakeAIDecision();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 center = (Vector2)transform.position + Vector2.right * 2f;
            Vector2 size = new Vector2(5f, 1f);
            Gizmos.DrawWireCube(center, size);
        }

        protected virtual void DealDamageAttack3(float damage, Vector2 boxSize, Vector2 offset)
        {
            if (player == null)
                return;

            Vector2 center = (Vector2)transform.position + offset;

            Collider2D hit = Physics2D.OverlapBox(center, boxSize, 0f, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                var playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage);
                }
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
