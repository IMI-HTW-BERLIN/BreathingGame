using UnityEngine;

namespace Enemies
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField] private GameObject awarenessIcon;
        [SerializeField] private SpriteRenderer spriteRendererAwareness;
        [SerializeField] private Sprite exclamationMark;
        [SerializeField] private Sprite questionMark;

        public void ShowAwareness()
        {
            spriteRendererAwareness.sprite = questionMark;
            awarenessIcon.gameObject.SetActive(true);
        }

        public void HideAwareness() => awarenessIcon.SetActive(false);

        public void ShowNoticed()
        {
            spriteRendererAwareness.sprite = exclamationMark;
            awarenessIcon.SetActive(true);
        }
    }
}