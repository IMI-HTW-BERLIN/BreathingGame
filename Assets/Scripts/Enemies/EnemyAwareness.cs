using System;
using PlayerBehaviour;
using Settings;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyAwareness : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float noticedDistance;
        [SerializeField] private LayerMask rayMask;


        public event Action<Player> OnPlayerFound;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Player player))
                return;

            LookForPlayer(player);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Player player))
                return;

            LookForPlayer(player);
        }

        /// <summary>
        /// Looks for the player by casting a ray. If hit and in reach -> <see cref="PlayerFound"/>.
        /// </summary>
        private void LookForPlayer(Player player)
        {
            Vector2 direction = player.transform.position - transform.position;

            if (!Physics2D.Raycast(transform.position, direction, 1000f, rayMask).collider
                .CompareTag(Consts.Tags.TAG_PLAYER))
                return;

            if (!(Vector2.Distance(player.transform.position, transform.position) <= noticedDistance))
                return;

            OnPlayerFound?.Invoke(player);
        }
    }
}