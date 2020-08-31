using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button btnPlay;
        [SerializeField] private TMP_InputField inputArduinoPortName;
        [SerializeField] private Button btnExit;

        private void Awake()
        {
            btnPlay.onClick.AddListener(OnPlay);
            btnExit.onClick.AddListener(OnExit);
        }

        private void OnPlay()
        {
            GameManager.Instance.LoadNextLevel();
            if (!string.IsNullOrEmpty(inputArduinoPortName.text))
                BreathingManager.Instance.StartArduinoReading(inputArduinoPortName.text);
        }

        private void OnExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}