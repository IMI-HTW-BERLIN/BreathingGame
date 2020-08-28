using Managers;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button btnPlay;
        [SerializeField] private Button btnExit;

        private void Awake()
        {
            btnPlay.onClick.AddListener(OnPlay);
            btnExit.onClick.AddListener(OnExit);
        }

        private void OnPlay() => GameManager.Instance.LoadScene(Consts.Scenes.LEVEL_1);

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