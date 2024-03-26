using Assets.Project.Scripts;
using UnityEngine;

namespace Assets.Scripts {
    /*
    public class Die : State {
        Animator animator;
        AudioData data;

        BoxCollider2D[] cols;
        public override void Enter(AbstractStateController machine) {
            data = FindObjectOfType<AudioData>();
            animator = GetComponent<Animator>();
            animator.Play("Death");
            SoundManager.PlaySound(data.death);
            cols = gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D col in cols) {
                col.enabled = false;
            }
        }

        
        public override void HandlePhysics() {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "End") {
                gameObject.SetActive(false);
            }
        }
        
    }
    */
    public class PlayerDeathState : AbstractDeathState {
        protected override void Awake() {
            base.Awake();
            _deathAnimation = "Death";
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            RemoveColliders(_cols);
        }
    }
}
