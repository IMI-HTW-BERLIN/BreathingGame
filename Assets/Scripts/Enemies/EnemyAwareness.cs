using System;
using PlayerBehaviour;
using Settings;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyAwareness : MonoBehaviour
    {
        [SerializeField] private LayerMask rayMask;
        public event Action<Player> OnPlayerFound;

        private bool _playerFound;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Player player) || _playerFound)
                return;

            _playerFound = true;
            LookForPlayer(player);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Player player) || _playerFound)
                return;

            _playerFound = true;
            LookForPlayer(player);
        }

        /// <summary>
        /// Looks for the player by casting a ray. If hit <see cref="OnPlayerFound"/> will be invoked.
        /// </summary>
        private void LookForPlayer(Player player)
        {
            Vector2 direction = player.transform.position - transform.position;

            if (!Physics2D.Raycast(transform.position, direction, 1000f, rayMask).collider
                .CompareTag(Consts.Tags.TAG_PLAYER))
                return;

            OnPlayerFound?.Invoke(player);
        }
    }
}