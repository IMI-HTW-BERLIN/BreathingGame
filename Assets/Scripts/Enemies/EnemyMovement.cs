using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private Transform pathStart;
        [SerializeField] private float distanceToNextWaypoint;

        [Header("Movement")] [SerializeField] private float movementSpeed;
        [SerializeField] private bool canMoveVertical;

        [SerializeField] private List<MovementData> movementData;

        public bool ReachedEndOfPath
        {
            get => _reachedEndOfPath;
            private set
            {
                if (value == _reachedEndOfPath)
                    return;
                _reachedEndOfPath = value;
                if (value && PatrolCheckpoints)
                    StartCoroutine(MoveToNextCheckpoint(movementData[_currentCheckpoint % movementData.Count]));
            }
        }

        public bool PatrolCheckpoints
        {
            get => _patrolCheckpoints;
            set
            {
                if (_patrolCheckpoints == value)
                    return;
                _patrolCheckpoints = value;
                if (value)
                    return;
                StopAllCoroutines();
                _seeker.CancelCurrentPathRequest();
                _currentWaypoint = 0;
            }
        }

        private Seeker _seeker;
        private Path _path;
        private Rigidbody2D _rb;

        private int _currentWaypoint;
        private int _currentCheckpoint;
        private bool _reachedEndOfPath;
        private bool _patrolCheckpoints = true;

        private Vector2 Position => pathStart == null ? transform.position : pathStart.position;

        private void Awake()
        {
            _seeker = GetComponent<Seeker>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start() => StartCoroutine(MoveToNextCheckpoint(movementData[0]));

        private void FixedUpdate()
        {
            if (_path == null)
                return;

            if (_currentWaypoint >= _path.vectorPath.Count)
            {
                ReachedEndOfPath = true;
                return;
            }

            ReachedEndOfPath = false;

            Vector2 direction = ((Vector2) _path.vectorPath[_currentWaypoint] - Position).normalized;
            Vector2 newVelocity;
            if (canMoveVertical)
                newVelocity = direction * (movementSpeed * Time.deltaTime);
            else
                newVelocity = new Vector2(Math.Sign(direction.x) * movementSpeed * Time.deltaTime, _rb.velocity.y);

            _rb.velocity = newVelocity;

            if (Vector2.Distance(Position, _path.vectorPath[_currentWaypoint]) < distanceToNextWaypoint)
                _currentWaypoint++;
        }

        public void MoveTo(Vector2 position, bool ignoreCanMoveVertical = false)
        {
            if (!_seeker.IsDone())
            {
                _seeker.CancelCurrentPathRequest();
                _currentWaypoint = 0;
            }

            Vector2 endPosition;
            if (canMoveVertical || ignoreCanMoveVertical)
                endPosition = position;
            else
                endPosition = new Vector2(position.x, _rb.position.y);

            _seeker.StartPath(Position, endPosition, path =>
            {
                _path = path;
                _currentWaypoint = 0;
            });
        }

        private IEnumerator MoveToNextCheckpoint(MovementData data)
        {
            yield return new WaitForSeconds(data.waitTime);
            MoveTo(data.endPosition, true);
            _currentCheckpoint++;
        }

        [Serializable]
        private struct MovementData
        {
            public float waitTime;
            public Vector3 endPosition;
        }
    }
}