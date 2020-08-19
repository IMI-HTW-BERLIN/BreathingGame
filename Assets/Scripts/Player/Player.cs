using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private ParticleSystem psSoundWaveRing;
        [SerializeField] private ParticleSystem psSoundWaveInside;
        [SerializeField] private float psLifetimeFactor;

        private InputActions _inputActions;

        private void Awake()
        {
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.PlayerControls.Fire.performed += OnFire;
            BreathingManager.Instance.OnBreathingChange += OnBreathingChange;
        }

        private void OnBreathingChange(bool isBreathingIn, float absDelta)
        {
            ParticleSystem.MainModule ringMainModule = psSoundWaveRing.main;
            ringMainModule.startLifetime = absDelta * psLifetimeFactor;

            ParticleSystem.MainModule insideMainModule = psSoundWaveInside.main;
            insideMainModule.startLifetime = absDelta * psLifetimeFactor;

            psSoundWaveRing.Play();
            psSoundWaveInside.Play();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
            _inputActions.PlayerControls.Fire.performed -= OnFire;
        }

        private void OnFire(InputAction.CallbackContext obj)
        {
        }
    }
}