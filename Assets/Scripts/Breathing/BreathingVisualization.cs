using System;
using Managers;
using UnityEngine;

namespace Breathing
{
    public class BreathingVisualization : MonoBehaviour
    {
        [SerializeField] private string arduinoPort;

        private void Start()
        {
            BreathingManager.Instance.StartArduinoReading(arduinoPort);
        }
    }
}
