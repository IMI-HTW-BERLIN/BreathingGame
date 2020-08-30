using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private Button btnRetry;
        [SerializeField] private Button btnExit;

        private void Awake()
        {
            btnRetry.onClick.AddListener(OnRetry);
            btnExit.onClick.AddListener(OnExit);
        }

        private void OnExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnRetry() => GameManager.Instance.ReloadLevel();
    }
}