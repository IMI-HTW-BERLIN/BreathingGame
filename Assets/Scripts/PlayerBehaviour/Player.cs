using System.Collections;
using System.Collections.Generic;
using LevelObjects;
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
        [SerializeField] private GameObject playerGfx;
        [SerializeField] private float killAnimationDelay;

        [Header("Particles")] [SerializeField] private ParticleSystem psSoundWaveRing;
        [SerializeField] private float psLifetimeFactor;
        [SerializeField] private float debugSoundWaveRange;

        [Header("Hiding")] [SerializeField] private string playerLayer;
        [SerializeField] private string enemyLayer;

        public bool IsHidden { get; private set; }
        public bool IsGrounded { get; private set; }

        private InputActions _playerInput;
        private Rigidbody2D _rb;
        private Interactable _currentInteractable;

        private readonly List<Vector2> _emissionPositions = new List<Vector2>();

        private static readonly int AnimatorSpeed = Animator.StringToHash("Speed");
        private static readonly int AnimatorVerticalVelocity = Animator.StringToHash("VerticalVelocity");
        private static readonly int AnimatorIsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int AnimatorIsDead = Animator.StringToHash("IsDead");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _playerInput = new InputActions();
        }

        private void OnEnable()
        {
            BreathingManager.Instance.OnBreathingChange += OnBreathingChange;

            _playerInput.Enable();

            _playerInput.Interaction.Fire.performed += OnFire;
            _playerInput.Interaction.Interact.performed += OnInteract;
            _playerInput.Movement.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            BreathingManager.Instance.OnBreathingChange -= OnBreathingChange;

            _playerInput.Disable();

            _playerInput.Interaction.Fire.performed -= OnFire;
            _playerInput.Interaction.Interact.performed -= OnInteract;
            _playerInput.Movement.Jump.performed -= OnJump;
        }

        private void FixedUpdate()
        {
            IsGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
            animator.SetBool(AnimatorIsGrounded, IsGrounded);

            float movement = _playerInput.Movement.Move.ReadValue<float>();

            float velocity = movement * movementSpeed * Time.deltaTime;
            _rb.velocity = new Vector2(velocity, _rb.velocity.y);

            // Animation
            animator.SetFloat(AnimatorSpeed, Mathf.Abs(movement));
            animator.SetFloat(AnimatorVerticalVelocity, _rb.velocity.y);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Interactable interactable))
                return;

            _currentInteractable = interactable;
            interactable.ShowUI();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Interactable interactable))
                return;

            _currentInteractable = null;
            interactable.HideUI();
        }

        public Vector2 GetLastParticlePosition() =>
            _emissionPositions.Count > 0 ? _emissionPositions[0] : (Vector2) transform.position;

        public void DisableInput() => _playerInput.Disable();

        public void Kill()
        {
            _playerInput.Disable();
            StartCoroutine(Coroutines.WaitForSeconds(killAnimationDelay,
                () => animator.SetBool(AnimatorIsDead, true)));

            GameManager.Instance.ShowGameOver();
        }

        public void HidePlayer()
        {
            playerGfx.SetActive(false);
            IsHidden = true;
            _playerInput.Movement.Disable();
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), LayerMask.NameToLayer(enemyLayer),
                true);
        }

        public void ShowPlayer(bool enableMovement = true)
        {
            playerGfx.SetActive(true);
            IsHidden = false;
            if (enableMovement)
                _playerInput.Movement.Enable();
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), LayerMask.NameToLayer(enemyLayer),
                false);
        }

        private void OnBreathingChange(bool isBreathingIn, float absDelta) => CreateSoundWave(absDelta);

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (!IsGrounded)
                return;
            _rb.AddForce(Vector2.up * jumpForce);
        }

        private void OnFire(InputAction.CallbackContext obj) => CreateSoundWave(debugSoundWaveRange);

        private void OnInteract(InputAction.CallbackContext obj)
        {
            if (!_currentInteractable)
                return;

            _currentInteractable.Interact(this);
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