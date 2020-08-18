using System;
using UnityEngine;

namespace Arduino
{
    public class TemperatureData
    {
        public int ID { get; private set; }
        public float Temperature { get; private set; }

        private const char SEPARATOR = '&';
        
        /// <summary>
        /// Requires string in following style:
        /// <code>
        /// id-temperature
        /// </code>
        /// </summary>
        /// <param name="arduinoString"></param>
        /// <returns></returns>
        public static TemperatureData FromArduinoString(string arduinoString)
        {
            string[] data = arduinoString.Split(SEPARATOR);
            if (data.Length != 2 || string.IsNullOrEmpty(data[0]) || string.IsNullOrEmpty(data[0]))
                return null;
            return new TemperatureData
            {
                ID = Convert.ToInt32(data[0]),
                Temperature = Convert.ToSingle(data[1])
            };

        }
    }
}