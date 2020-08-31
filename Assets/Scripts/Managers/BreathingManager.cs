using Arduino;
using Managers.Abstract;
using UnityEngine;
using Utils;

namespace Managers
{
    public class BreathingManager : Singleton<BreathingManager>
    {
        [SerializeField] private ArduinoReader arduinoReader;

        [Header("Breathing Settings")] [SerializeField]
        private float deltaDifferenceForBreathing;

        [SerializeField] private float minDeltaForBreathingChange;

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

        public LimitedList<float> AbsDeltas { get; } = new LimitedList<float>(4);

        public event TemperatureReading OnTemperatureRead;
        public event BreathingChange OnBreathingChange;
        public event HoldingBreath OnHoldingBreath;

        public delegate void TemperatureReading(float temperature);

        public delegate void BreathingChange(bool isBreathingIn, float absDelta);

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

        public void StartArduinoReading(string arduinoPort) => arduinoReader.StartReading(arduinoPort);

        private void CheckBreathingType()
        {
            float delta = _currentTemperature - _lastTemperature;
            float absDelta = Mathf.Abs(delta);
            AbsDeltas.Add(absDelta);

            // Check if delta is negative -> breathing in due to cooler air.
            bool breathingIn = delta < 0;
            if (_isBreathingIn != breathingIn && absDelta >= minDeltaForBreathingChange)
            {
                _isBreathingIn = breathingIn;
                OnBreathingChange?.Invoke(breathingIn, absDelta);
            }


            if (!AbsDeltas.IsFull)
                return;
            // If temperature is not changing much, no breathing in or out.
            IsHoldingBreath = AbsDeltas.Average() < deltaDifferenceForBreathing;
        }
    }
}