using Managers;
using PlayerBehaviour;
using UnityEngine;

namespace LevelObjects
{
    public class ExitDoor : Interactable
    {
        [SerializeField] private GameObject closedDoor;
        [SerializeField] private GameObject openDoor;

        private void OnEnable()
        {
            GameManager.Instance.OnAllCrystalsCollected += OnAllCrystalsCollected;
            CanInteract = false;
        }

        private void OnDisable() => GameManager.Instance.OnAllCrystalsCollected -= OnAllCrystalsCollected;

        private void OnAllCrystalsCollected()
        {
            closedDoor.SetActive(false);
            openDoor.SetActive(true);
            CanInteract = true;
        }

        public override void Interact(Player player) => GameManager.Instance.LoadNextLevel();
    }
}