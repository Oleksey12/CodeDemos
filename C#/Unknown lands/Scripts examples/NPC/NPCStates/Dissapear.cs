using UnityEngine;

namespace Assets.Scripts.NPC.NPCStates {
    public class Dissapear : State {
        Animator animator;

        public override void Enter(AbstractStateController machine) {
            animator = GetComponent<Animator>();
        }

        public override void HandlePhysics() {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Delete") {
                gameObject.SetActive(false);
            }
        }
    }
}
