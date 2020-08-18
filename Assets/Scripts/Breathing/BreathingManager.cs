using System;
using System.Collections.Generic;
using System.Linq;
using Graph;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Breathing
{
    public class BreathingManager : Singleton<BreathingManager>
    {
        [SerializeField] private GraphPlotter graphPlotter;
        [SerializeField] private Image imgBreathingType;
        [SerializeField] private Image imgBreathing;
        [SerializeField] private int warmUpAmount;

        [Header("Breathing Settings")] [SerializeField]
        private float deltaDifferenceForBreathing;


        private readonly List<float> _temperatureValues = new List<float>();
        private readonly LimitedList<float> _deltas = new LimitedList<float>(4);

        private int _currentWarmUpIndex;
        private bool _avgCalculated;

        public void AddTemperature(float temperature)
        {
            _temperatureValues.Add(temperature);
            graphPlotter.AddNextPoint(0, temperature);
            
            if (_currentWarmUpIndex < warmUpAmount)
            {
                _currentWarmUpIndex++;
                return;
            }
            CheckBreathingType();
        }

        private void CheckBreathingType()
        {
            float delta = _temperatureValues[_temperatureValues.Count - 1] -
                          _temperatureValues[_temperatureValues.Count - 2];

            imgBreathing.color = delta > 0 ? Color.green : Color.red;

            _deltas.Add(Mathf.Abs(delta));
            
            if(!_deltas.IsFull)
                return;

            imgBreathingType.color = _deltas.AllSmallerThan(deltaDifferenceForBreathing) ? Color.black : Color.white;
        }
    }
}