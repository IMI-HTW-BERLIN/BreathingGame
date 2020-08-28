using System;
using System.Collections.Generic;
using LevelObjects;
using Managers.Abstract;
using PlayerBehaviour;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Animator gameOverAnimator;
        [SerializeField] private float gameOverScreenDelayTime;

        [SerializeField] private Animator sceneTransitionAnimator;
        [SerializeField] private float sceneTransitionTime;

        public event Action OnAllCrystalsCollected;

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
        private List<Crystal> _crystalsToCollect = new List<Crystal>();

        private static readonly int GameOverIn = Animator.StringToHash("GameOver_IN");
        private static readonly int TransitionIn = Animator.StringToHash("Transition_IN");

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;


        public void ShowGameOver() =>
            StartCoroutine(Coroutines.WaitForSeconds(gameOverScreenDelayTime,
                () =>
                {
                    gameOverAnimator.gameObject.SetActive(true);
                    gameOverAnimator.SetTrigger(GameOverIn);
                }));

        public void LoadScene(int sceneIndex)
        {
            sceneTransitionAnimator.SetTrigger(TransitionIn);
            StartCoroutine(Coroutines.WaitForSeconds(sceneTransitionTime,
                () => SceneManager.LoadScene(sceneIndex)));
        }

        public void CrystalCollected()
        {
            if (_crystalsToCollect.TrueForAll(crystal => crystal.IsCollected))
                OnAllCrystalsCollected?.Invoke();
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            sceneTransitionAnimator.gameObject.SetActive(true);
            _crystalsToCollect = new List<Crystal>(FindObjectsOfType<Crystal>());
        }

        private void OnSceneUnloaded(Scene arg0)
        {
            gameOverAnimator.gameObject.SetActive(false);
            sceneTransitionAnimator.gameObject.SetActive(false);
        }
    }
}