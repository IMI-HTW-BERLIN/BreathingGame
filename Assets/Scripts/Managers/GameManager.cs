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
        [SerializeField] private Animator animator;
        [SerializeField] private float showGameOverScreenDelay;

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

        private static readonly int FadeIn = Animator.StringToHash("FadeIn");
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;


        public void ShowGameOver() =>
            StartCoroutine(Coroutines.WaitForSeconds(showGameOverScreenDelay, () => animator.SetTrigger(FadeIn)));

        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
            animator.SetTrigger(FadeOut);
        }

        public void CrystalCollected()
        {
            if (_crystalsToCollect.TrueForAll(crystal => crystal.IsCollected))
                OnAllCrystalsCollected?.Invoke();
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) =>
            _crystalsToCollect = new List<Crystal>(FindObjectsOfType<Crystal>());
    }
}