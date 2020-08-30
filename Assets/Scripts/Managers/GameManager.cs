using System;
using System.Collections.Generic;
using LevelObjects;
using Managers.Abstract;
using PlayerBehaviour;
using UI;
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

        [SerializeField] private LevelGoalScreen levelGoalScreen;

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

        private int _currentLevel;

        private static readonly int GameOverIn = Animator.StringToHash("GameOver_IN");
        private static readonly int TransitionIn = Animator.StringToHash("Transition_IN");

        /// <summary>
        /// Sets <see cref="_currentLevel"/> to the correct index. Needed when skipping the MainMenu scene.
        /// </summary>
        private void Start() => _currentLevel = SceneManager.GetActiveScene().buildIndex;

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

        public void LoadNextLevel()
        {
            if (_currentLevel + 1 >= SceneManager.sceneCountInBuildSettings)
            {
                LoadScene(0);
                _currentLevel = 0;
                return;
            }

            _currentLevel++;
            LoadScene(_currentLevel);
        }

        public void ReloadLevel() => LoadScene(_currentLevel);

        public void CrystalCollected()
        {
            if (_crystalsToCollect.TrueForAll(crystal => crystal.IsCollected))
                OnAllCrystalsCollected?.Invoke();
        }

        private void LoadScene(int sceneIndex)
        {
            sceneTransitionAnimator.SetTrigger(TransitionIn);
            StartCoroutine(Coroutines.WaitForSeconds(sceneTransitionTime,
                () => SceneManager.LoadScene(sceneIndex)));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            _crystalsToCollect = new List<Crystal>(FindObjectsOfType<Crystal>());
            sceneTransitionAnimator.gameObject.SetActive(true);
            if (_crystalsToCollect.Count == 0 || scene.buildIndex == 0)
                return;
            levelGoalScreen.ShowCrystalsToCollect(_crystalsToCollect);
            levelGoalScreen.gameObject.SetActive(true);
        }

        private void OnSceneUnloaded(Scene arg0)
        {
            gameOverAnimator.gameObject.SetActive(false);
            sceneTransitionAnimator.gameObject.SetActive(false);
            levelGoalScreen.gameObject.SetActive(false);
        }
    }
}