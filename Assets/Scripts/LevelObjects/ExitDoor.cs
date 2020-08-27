using Managers;
using UnityEngine;

namespace LevelObjects
{
    public class ExitDoor : MonoBehaviour
    {
        [SerializeField] private GameObject closedDoor;
        [SerializeField] private GameObject openDoor;

        private void OnEnable() => GameManager.Instance.OnAllCrystalsCollected += OnAllCrystalsCollected;

        private void OnDisable() => GameManager.Instance.OnAllCrystalsCollected -= OnAllCrystalsCollected;

        private void OnAllCrystalsCollected()
        {
            closedDoor.SetActive(false);
            openDoor.SetActive(true);
        }
    }
}