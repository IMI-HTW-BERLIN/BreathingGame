using System;
using Managers;
using UnityEngine;

namespace Graph
{
    public class GraphPlotter : MonoBehaviour
    {
        [Header("Canvas that defines the size")] [SerializeField]
        private Canvas drawArea;

        [Space(10)] [SerializeField] private float centerPointRatio;
        [Space(10)] [SerializeField] private float max;
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
            float value = Mathf.Clamp01((yValue - min) / (max - min)) *
                          ((RectTransform) drawArea.transform).rect.height;
            graph.lineRenderer.positionCount = graph.PointIndex + 1;
            graph.lineRenderer.SetPosition(graph.PointIndex, new Vector3(graph.PointIndex * spaceFactor, value));
            graph.PointIndex++;

            if (graph.lineRenderer.transform.localPosition.x + graph.PointIndex * spaceFactor >
                ((RectTransform) drawArea.transform).rect.width * centerPointRatio)
                graph.lineRenderer.transform.localPosition -= new Vector3(spaceFactor, 0);
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