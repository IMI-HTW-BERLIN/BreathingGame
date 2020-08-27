using Managers;
using PlayerBehaviour;
using UnityEngine;

namespace LevelObjects
{
    [RequireComponent(typeof(Collider2D))]
    public class Crystal : MonoBehaviour
    {
        public bool IsCollected { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Player _))
                return;

            IsCollected = true;
            gameObject.SetActive(false);
            GameManager.Instance.CrystalCollected();
        }
    }
}