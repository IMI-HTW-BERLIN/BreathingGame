using PlayerBehaviour;
using UnityEngine;
using Utils;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider2D))]
    public class Hideout : MonoBehaviour
    {
        [SerializeField] private GameObject interactableUI;
        [SerializeField] private Animator animator;
        [SerializeField] private float playerHideDelay;


        private static readonly int Explode = Animator.StringToHash("Explode");

        public void ShowUI() => interactableUI.SetActive(true);
        public void HideUI() => interactableUI.SetActive(false);

        public void Hide(Player player)
        {
            animator.SetTrigger(Explode);
            StartCoroutine(Coroutines.WaitForSeconds(playerHideDelay, player.HidePlayer));
        }

        public void UnHide(Player player)
        {
            animator.SetTrigger(Explode);
            StartCoroutine(Coroutines.WaitForSeconds(playerHideDelay, player.ShowPlayer));
        }
    }
}