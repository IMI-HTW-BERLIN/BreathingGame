using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using Managers;
using UnityEngine;

namespace Arduino
{
    public class ArduinoReader : MonoBehaviour
    {
        [SerializeField] private string portName;
        [SerializeField] private int baudRate = 9600;

        private SerialPort _serialPort;

        private static readonly char[] Separator = {'\r', '\n'};

        private void Awake()
        {
            _serialPort = new SerialPort(portName, baudRate);
        }

        private void OnEnable()
        {
            _serialPort.Open();
            StartCoroutine(ReadNextLine());
        }

        private void OnDisable() => _serialPort.Close();

        [SuppressMessage("ReSharper", "FunctionRecursiveOnAllPaths")]
        private IEnumerator ReadNextLine()
        {
            yield return new WaitUntil(() => _serialPort.BytesToRead > 0);
            foreach (string data in _serialPort.ReadLine().Split(Separator))
            {
                BreathingManager.Instance.AddTemperature(Convert.ToSingle(data));
            }

            yield return ReadNextLine();
        }
    }
}