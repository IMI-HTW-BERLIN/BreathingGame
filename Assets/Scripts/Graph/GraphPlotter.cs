using System;
using Managers;
using UnityEngine;

namespace Graph
{
    public class GraphPlotter : MonoBehaviour
    {
        [SerializeField] private float max;
        [SerializeField] private float min;
        [SerializeField] private float spaceFactor;
        [Space(10)] [SerializeField] private Graph graph;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;

            graph.lineRenderer.startColor = graph.graphColor;
            graph.lineRenderer.endColor = graph.graphColor;
        }

        private void OnEnable() => BreathingManager.Instance.OnTemperatureRead += AddNextPoint;

        private void OnDisable() => BreathingManager.Instance.OnTemperatureRead -= AddNextPoint;

        private void AddNextPoint(float yValue)
        {
            float value = Mathf.Clamp01((yValue - min) / (max - min)) * Screen.height / 100f;
            graph.lineRenderer.positionCount = graph.PointIndex + 1;
            graph.lineRenderer.SetPosition(graph.PointIndex, new Vector3(graph.PointIndex * spaceFactor, value));
            graph.PointIndex++;

            if (_camera.transform.position.x < graph.PointIndex * spaceFactor - Screen.width / 100f * 0.75f)
                _camera.transform.position += new Vector3(spaceFactor, 0);
        }

        [Serializable]
        public class Graph
        {
            public Color graphColor;
            public LineRenderer lineRenderer;
            public int PointIndex { get; set; }
        }
    }
}