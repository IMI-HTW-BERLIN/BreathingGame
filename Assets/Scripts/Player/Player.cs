using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
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

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

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

            if (_rb.velocity.x > 0.01f)
                playerSprite.flipX = false;
            else if (_rb.velocity.x < -0.01f)
                playerSprite.flipX = true;

            // Animation
            animator.SetFloat(Speed, Mathf.Abs(movement));
            animator.SetFloat(VerticalVelocity, _rb.velocity.y);
        }

        private void OnBreathingChange(bool isBreathingIn, float absDelta)
        {
            ParticleSystem.MainModule main = psSoundWaveRing.main;
            main.startLifetime = absDelta * psLifetimeFactor;

            psSoundWaveRing.Play();
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (!_isGrounded)
                return;
            _rb.AddForce(Vector2.up * jumpForce);
        }

        private void OnFire(InputAction.CallbackContext obj)
        {
            psSoundWaveRing.Play();
        }
    }
}