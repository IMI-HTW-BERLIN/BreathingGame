using Pathfinding;
using Settings;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyUI enemyUI;
        [SerializeField] private float triggerCooldown = 5f;
        [Header("Movement")] [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private float nextWaypointDistance = 3f;
        [SerializeField] private SpriteRenderer enemySprite;

        private Rigidbody2D _rb;

        // Pathfinding
        private Seeker _seeker;
        private Path _path;
        private int _currentPathNode;

        private float _cooldownTime;

        private void Awake()
        {
            _seeker = GetComponent<Seeker>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_rb.velocity.x > 0.01f)
                enemySprite.flipX = false;
            else if (_rb.velocity.x < -0.01f)
                enemySprite.flipX = true;

            if (_path == null)
                return;

            if (_currentPathNode >= _path.vectorPath.Count)
            {
                _path = null;
                return;
            }

            Vector2 direction = ((Vector2) _path.vectorPath[_currentPathNode] - _rb.position).normalized;
            Vector2 force = direction * (movementSpeed * Time.deltaTime * _rb.mass);

            _rb.AddForce(force);

            float distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentPathNode]);
            if (distance < nextWaypointDistance)
                _currentPathNode++;
        }

        private void OnParticleCollision(GameObject other)
        {
            float time = Time.time;
            if (!other.CompareTag(Consts.Tags.TAG_PLAYER))
                return;
            if (time < _cooldownTime + triggerCooldown)
                return;

            _cooldownTime = time;
            enemyUI.ShowAwareness();

            MoveToLocation(other.transform.position);
        }

        private void MoveToLocation(Vector2 position)
        {
            if (!_seeker.IsDone())
                return;

            _seeker.StartPath(_rb.position, position, path =>
            {
                if (path.error)
                    return;
                _path = path;
                _currentPathNode = 0;
            });
        }
    }
}