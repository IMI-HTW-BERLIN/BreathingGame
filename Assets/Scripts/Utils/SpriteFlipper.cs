using UnityEngine;

namespace Utils
{
    public class SpriteFlipper : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rigidbodyForMovement;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool flipGameObjectInstead;

        private void Update()
        {
            if (rigidbodyForMovement.velocity.x > 0.01f)
                if (flipGameObjectInstead)
                    transform.localScale = new Vector3(1, 1, 1);
                else
                    spriteRenderer.flipX = false;
            else if (rigidbodyForMovement.velocity.x < -0.01f)
                if (flipGameObjectInstead)
                    transform.localScale = new Vector3(-1, 1, 1);
                else
                    spriteRenderer.flipX = true;
        }
    }
}