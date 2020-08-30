using PlayerBehaviour;
using UnityEngine;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private GameObject interactableUI;

        protected bool CanInteract = true;

        public void ShowUI()
        {
            if (CanInteract)
                interactableUI.SetActive(true);
        }

        public void HideUI()
        {
            if (CanInteract)
                interactableUI.SetActive(false);
        }

        public abstract void Interact(Player player);
    }
}