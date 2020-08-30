using PlayerBehaviour;
using UnityEngine;
using Utils;

namespace LevelObjects
{
    public class Hideout : Interactable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float playerHideDelay;

        private bool _playerIsHidden;
        private static readonly int Explode = Animator.StringToHash("Explode");


        public override void Interact(Player player)
        {
            if (_playerIsHidden)
            {
                animator.SetTrigger(Explode);
                StartCoroutine(Coroutines.WaitForSeconds(playerHideDelay, () => player.ShowPlayer()));
                _playerIsHidden = false;
            }
            else
            {
                animator.SetTrigger(Explode);
                StartCoroutine(Coroutines.WaitForSeconds(playerHideDelay, player.HidePlayer));
                _playerIsHidden = true;
            }
        }
    }
}