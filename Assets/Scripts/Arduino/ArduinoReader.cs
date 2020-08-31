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
        [SerializeField] private int baudRate = 9600;

        private SerialPort _serialPort;

        private static readonly char[] Separator = {'\r', '\n'};

        private void OnDisable() => _serialPort?.Close();

        public void StartReading(string portName)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.Open();
            StartCoroutine(ReadNextLine());
        }

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