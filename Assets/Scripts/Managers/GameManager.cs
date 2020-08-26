using Managers.Abstract;
using PlayerBehaviour;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float animationTime;

        public Player Player
        {
            get
            {
                if (_player == null)
                    _player = FindObjectOfType<Player>();
                return _player;
            }
        }

        private Player _player;

        private static readonly int FadeIn = Animator.StringToHash("FadeIn");
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");

        public void ShowGameOver() => animator.SetTrigger(FadeIn);

        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
            animator.SetTrigger(FadeOut);
        }
    }
}