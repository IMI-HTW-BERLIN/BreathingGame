using Managers;
using PlayerBehaviour;
using Settings;
using UnityEngine;
using Utils;

namespace Enemies
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyUI enemyUI;
        [SerializeField] private EnemyMovement enemyMovement;

        [Header("Awareness")] [SerializeField] private float triggerCooldown;

        [SerializeField] private float awarenessTime;
        [SerializeField] private float noticedDistance;
        [SerializeField] private LayerMask rayMask;

        [Header("Animation")] [SerializeField] private Animator animator;

        private Rigidbody2D _rb;
        private float _cooldownTime;
        private bool _isAttacking;
        private bool _hasKilled;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack = Animator.StringToHash("Attack");


        protected virtual void Awake() => _rb = GetComponent<Rigidbody2D>();

        protected virtual void Update()
        {
            animator.SetFloat(Speed, Mathf.Abs(_rb.velocity.x));
            LookForPlayer();
            if (_isAttacking && !_hasKilled && enemyMovement.ReachedEndOfPath)
                AttackPlayer();
        }

        /// <summary>
        /// When a sound wave hits, move to the origin of it and show 'Awareness'.
        /// </summary>
        protected virtual void OnParticleCollision(GameObject other)
        {
            float time = Time.time;
            if (!other.CompareTag(Consts.Tags.TAG_PLAYER))
                return;
            if (time < _cooldownTime + triggerCooldown)
                return;

            _cooldownTime = time;
            enemyUI.ShowAwareness();
            enemyMovement.PatrolCheckpoints = false;
            StopAllCoroutines();
            StartCoroutine(Coroutines.WaitForSeconds(awarenessTime, () =>
            {
                enemyUI.HideAwareness();
                enemyMovement.PatrolCheckpoints = true;
            }));
            enemyMovement.MoveTo(GameManager.Instance.Player.GetLastParticlePosition());
        }

        /// <summary>
        /// Looks for the player by casting a ray. If hit and in reach -> <see cref="PlayerFound"/>.
        /// </summary>
        private void LookForPlayer()
        {
            Player player = GameManager.Instance.Player;
            Vector2 direction = player.transform.position - transform.position;

            if (!Physics2D.Raycast(transform.position, direction, 1000f, rayMask).collider
                .CompareTag(Consts.Tags.TAG_PLAYER))
                return;

            if (!(Vector2.Distance(player.transform.position, transform.position) <= noticedDistance))
                return;

            PlayerFound(player);
        }

        /// <summary>
        /// Disables player movement and moves to the player to attack him.
        /// </summary>
        /// <param name="player">The player that was found (only for script reference)</param>
        private void PlayerFound(Player player)
        {
            if (_isAttacking || player.IsHidden)
                return;
            _isAttacking = true;
            enemyMovement.PatrolCheckpoints = false;
            StopAllCoroutines();
            enemyUI.ShowNoticed();
            player.DisableMovement();
            enemyMovement.MoveTo(player.transform.position);
        }

        /// <summary>
        /// Starts the attack animation and kills the player.
        /// </summary>
        private void AttackPlayer()
        {
            enemyUI.HideAwareness();
            animator.SetTrigger(Attack);
            GameManager.Instance.Player.Kill();
            _hasKilled = true;
        }
    }
}