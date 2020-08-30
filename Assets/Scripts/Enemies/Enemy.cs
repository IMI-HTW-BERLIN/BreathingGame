using Managers;
using PlayerBehaviour;
using Settings;
using UnityEngine;
using Utils;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private EnemyUI enemyUI;
        [SerializeField] private EnemyMovement enemyMovement;
        [Header("Awareness")] [SerializeField] private EnemyAwareness enemyAwareness;
        [SerializeField] private float triggerCooldown;
        [SerializeField] private float awarenessTime;


        [Header("Animation")] [SerializeField] private Animator animator;
        private float _cooldownTime;
        private bool _isAttacking;
        private bool _hasKilled;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack = Animator.StringToHash("Attack");

        protected void OnEnable() => enemyAwareness.OnPlayerFound += PlayerFound;
        protected void OnDisable() => enemyAwareness.OnPlayerFound -= PlayerFound;

        protected virtual void Update()
        {
            animator.SetFloat(Speed, Mathf.Abs(rb.velocity.x));
            if (_isAttacking && !_hasKilled && enemyMovement.ReachedEndOfPath)
                AttackPlayer();
        }

        /// <summary>
        /// When a sound wave hits, move to the origin of it and show 'Awareness'.
        /// </summary>
        protected virtual void OnParticleCollision(GameObject other)
        {
            float time = Time.time;
            if (!other.CompareTag(Consts.Tags.TAG_PLAYER) || time < _cooldownTime + triggerCooldown || _isAttacking ||
                _hasKilled)
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
        /// Disables player movement and moves to the player to attack him.
        /// </summary>
        /// <param name="player">The player that was found (only for script reference)</param>
        private void PlayerFound(Player player)
        {
            if (_isAttacking || player.IsHidden)
                return;
            enemyMovement.PatrolCheckpoints = false;
            StopAllCoroutines();
            enemyUI.ShowNoticed();
            player.DisableMovement();
            StartCoroutine(Coroutines.WaitUntil(() => player.IsGrounded, () =>
            {
                _isAttacking = true;
                enemyMovement.MoveTo(player.transform.position);
            }));
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