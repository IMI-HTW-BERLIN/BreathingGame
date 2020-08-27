using System.Collections;
using System.Collections.Generic;
using LevelObjects;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private GameObject playerGfx;

        [Header("Particles")] [SerializeField] private ParticleSystem psSoundWaveRing;
        [SerializeField] private float psLifetimeFactor;

        [Header("Hiding")] [SerializeField] private string playerLayer;
        [SerializeField] private string enemyLayer;

        public bool IsHidden { get; private set; }

        private InputActions _inputActions;
        private Rigidbody2D _rb;
        private bool _isGrounded;
        private Hideout _currentHideout;

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
            _inputActions.Interaction.Fire.performed += OnFire;
            _inputActions.Interaction.Interact.performed += OnInteract;
            _inputActions.Movement.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            BreathingManager.Instance.OnBreathingChange -= OnBreathingChange;

            _inputActions.Disable();
            _inputActions.Interaction.Fire.performed -= OnFire;
            _inputActions.Interaction.Interact.performed -= OnInteract;
            _inputActions.Movement.Jump.performed -= OnJump;
        }

        private void FixedUpdate()
        {
            _isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
            animator.SetBool(IsGrounded, _isGrounded);

            float movement = _inputActions.Movement.Move.ReadValue<float>();

            float velocity = movement * movementSpeed * Time.deltaTime;
            _rb.velocity = new Vector2(velocity, _rb.velocity.y);

            // Animation
            animator.SetFloat(Speed, Mathf.Abs(movement));
            animator.SetFloat(VerticalVelocity, _rb.velocity.y);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Hideout hideout))
                return;

            _currentHideout = hideout;
            hideout.ShowUI();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Hideout hideout))
                return;

            _currentHideout = null;
            hideout.HideUI();
        }

        public Vector2 GetLastParticlePosition() =>
            _emissionPositions.Count > 0 ? _emissionPositions[0] : (Vector2) transform.position;

        public void DisableMovement() => _inputActions.Movement.Disable();

        public void Kill()
        {
            _inputActions.Disable();
            animator.SetBool(IsDead, true);
            GameManager.Instance.ShowGameOver();
        }

        public void HidePlayer()
        {
            playerGfx.SetActive(false);
            IsHidden = true;
            _inputActions.Movement.Disable();
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), LayerMask.NameToLayer(enemyLayer),
                true);
        }

        public void ShowPlayer()
        {
            playerGfx.SetActive(true);
            IsHidden = false;
            _inputActions.Movement.Enable();
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), LayerMask.NameToLayer(enemyLayer),
                false);
        }

        private void OnBreathingChange(bool isBreathingIn, float absDelta) => CreateSoundWave(absDelta);

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (!_isGrounded)
                return;
            _rb.AddForce(Vector2.up * jumpForce);
        }

        private void OnFire(InputAction.CallbackContext obj) => CreateSoundWave(0.3f);

        private void OnInteract(InputAction.CallbackContext obj)
        {
            if (!_currentHideout)
                return;
            if (IsHidden)
                _currentHideout.UnHide(this);
            else
                _currentHideout.Hide(this);
        }

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