using UnityEngine;

namespace Assets.Scripts {
    public class AnimationScripts {
        public static void HandleDirection(SpriteRenderer spriteRenderer, float horizontalSpeed) {
            if (!spriteRenderer.flipX && horizontalSpeed < 0) {
                spriteRenderer.flipX = true;
            } else if (spriteRenderer.flipX && horizontalSpeed > 0) {
                spriteRenderer.flipX = false;
            }
        }

        public static void SetAnimatorBoolean(Animator animator, string name, bool val) {
            animator.SetBool(name, val);
        }
    }
}
