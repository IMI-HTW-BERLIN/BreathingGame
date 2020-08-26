using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace PlayerBehaviour
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [Header("Controls")] [SerializeField] private float movementSpeed = 1000f;
        [SerializeField] private float jumpForce = 50f;

        [Header("Ground Check")] [SerializeField]
        private Transform groundCheck;

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance;

        [Header("Animation")] [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer playerSprite;

        [Header("Particles")] [SerializeField] private ParticleSystem psSoundWaveRing;
        [SerializeField] private float psLifetimeFactor;

        private InputActions _inputActions;
        private Rigidbody2D _rb;
        private bool _isGrounded;

        private readonly List<Vector2> _emissionPositions = new List<Vector2>();

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsDead = Animator.StringToHash("IsDead");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            BreathingManager.Instance.OnBreathingChange += OnBreathingChange;

            _inputActions.Enable();
            _inputActions.PlayerControls.Fire.performed += OnFire;
            _inputActions.PlayerControls.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            BreathingManager.Instance.OnBreathingChange -= OnBreathingChange;

            _inputActions.Disable();
            _inputActions.PlayerControls.Fire.performed -= OnFire;
        }

        private void FixedUpdate()
        {
            _isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
            animator.SetBool(IsGrounded, _isGrounded);

            float movement = _inputActions.PlayerControls.Movement.ReadValue<float>();

            float velocity = movement * movementSpeed * Time.deltaTime;
            _rb.velocity = new Vector2(velocity, _rb.velocity.y);

            // Animation
            animator.SetFloat(Speed, Mathf.Abs(movement));
            animator.SetFloat(VerticalVelocity, _rb.velocity.y);
        }

        public Vector2 GetLastParticlePosition() =>
            _emissionPositions.Count > 0 ? _emissionPositions[0] : (Vector2) transform.position;

        public void DisableMovement() => _inputActions.PlayerControls.Disable();

        public void Kill()
        {
            animator.SetBool(IsDead, true);
            StartCoroutine(Coroutines.WaitForSeconds(2f, () => GameManager.Instance.ShowGameOver()));
        }

        private void OnBreathingChange(bool isBreathingIn, float absDelta) => CreateSoundWave(absDelta);

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (!_isGrounded)
                return;
            _rb.AddForce(Vector2.up * jumpForce);
        }

        private void OnFire(InputAction.CallbackContext obj) => CreateSoundWave(0.3f);

        private void CreateSoundWave(float range)
        {
            float particleRange = range * psLifetimeFactor;
            ParticleSystem.MainModule main = psSoundWaveRing.main;
            main.startLifetime = particleRange;

            _emissionPositions.Add(psSoundWaveRing.transform.position);
            StartCoroutine(RemoveAfterDelay(particleRange));
            psSoundWaveRing.Play();
        }

        private IEnumerator RemoveAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _emissionPositions.RemoveAt(0);
        }
    }
}