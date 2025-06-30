using System.Collections;
using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.Enemy
{
    public class TrapDamage : MonoBehaviour
    {
        public float damagePerSecond = 20f;
        private Coroutine damageCoroutine;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(player));
            }
        }

        //private void OnTriggerStay2D(Collider2D other)
        //{
        //    PlayerHealth player = other.GetComponent<PlayerHealth>();
        //    if (player != null)
        //    {
        //        damageCoroutine = StartCoroutine(DealDamageOverTime(player));
        //    }
        //}

        private void OnTriggerExit2D(Collider2D other)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }

        private IEnumerator DealDamageOverTime(PlayerHealth player)
        {
            while (true)
            {
                player.TakeDamage(damagePerSecond);
                yield return new WaitForSeconds(1f); 
            }
        }
    }
}
