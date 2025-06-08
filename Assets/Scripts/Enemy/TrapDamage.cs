using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class TrapDamage : MonoBehaviour
    {
        public float damagePerSecond = 20f;

        private void OnTriggerStay2D(Collider2D other)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}
