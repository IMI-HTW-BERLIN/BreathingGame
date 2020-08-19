using Managers.Abstract;
using UnityEngine;
using Utils;

namespace Managers
{
    public class BreathingManager : Singleton<BreathingManager>
    {
        [Header("Breathing Settings")] [SerializeField]
        private float deltaDifferenceForBreathing;

        public bool IsHoldingBreath
        {
            get => _isHoldingBreath;
            private set
            {
                if (_isHoldingBreath != value)
                    OnHoldingBreath?.Invoke(value);
                _isHoldingBreath = value;
            }
        }

        public bool IsBreathingIn
        {
            get => _isBreathingIn;
            private set
            {
                if (_isBreathingIn != value)
                    OnBreathingChange?.Invoke(value);
                _isBreathingIn = value;
            }
        }

        public LimitedList<float> AbsDeltas { get; } = new LimitedList<float>(4);

        public event TemperatureReading OnTemperatureRead;
        public event BreathingChange OnBreathingChange;
        public event HoldingBreath OnHoldingBreath;

        public delegate void TemperatureReading(float temperature);

        public delegate void BreathingChange(bool isBreathingIn);

        public delegate void HoldingBreath(bool isHoldingBreath);


        private float _lastTemperature;
        private float _currentTemperature;
        private bool _isHoldingBreath;
        private bool _isBreathingIn;

        public void AddTemperature(float temperature)
        {
            _currentTemperature = temperature;
            CheckBreathingType();
            _lastTemperature = temperature;
            OnTemperatureRead?.Invoke(temperature);
        }

        private void CheckBreathingType()
        {
            float delta = _currentTemperature - _lastTemperature;
            // Check if delta is negative -> breathing in due to cooler air.
            IsBreathingIn = delta < 0;
            AbsDeltas.Add(Mathf.Abs(delta));

            if (!AbsDeltas.IsFull)
                return;
            // If temperature is not changing much, no breathing in or out.
            // IsHoldingBreath = AbsDeltas.AllSmallerThan(deltaDifferenceForBreathing);
            IsHoldingBreath = AbsDeltas.Average() < deltaDifferenceForBreathing;
        }
    }
}