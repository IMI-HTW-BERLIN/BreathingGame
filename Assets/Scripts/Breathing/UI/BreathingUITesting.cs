using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Breathing.UI
{
    public class BreathingUITesting : MonoBehaviour
    {
        [SerializeField] private Image imgBreathingType;
        [SerializeField] private Image imgHoldingBreath;

        private readonly Vector3 _flipHorizontal = new Vector3(1, -1, 1);
        private readonly Vector3 _normalScale = new Vector3(1, 1, 1);

        private void OnEnable()
        {
            BreathingManager.Instance.OnBreathingChange += OnBreathingChange;
            BreathingManager.Instance.OnHoldingBreath += OnHoldingBreathChange;
        }

        private void OnDisable()
        {
            BreathingManager.Instance.OnBreathingChange -= OnBreathingChange;
            BreathingManager.Instance.OnHoldingBreath -= OnHoldingBreathChange;
        }

        private void OnHoldingBreathChange(bool isHoldingBreath)
        {
            imgHoldingBreath.color = isHoldingBreath ? Color.black : Color.white;
        }

        private void OnBreathingChange(bool isBreathingIn, float absDelta)
        {
            imgBreathingType.transform.localScale = isBreathingIn ? _normalScale : _flipHorizontal;
        }
    }
}