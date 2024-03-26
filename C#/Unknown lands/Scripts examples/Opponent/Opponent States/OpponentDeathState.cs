using Assets.Project.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    /*
    public class OpponentDie : State {
        
        AudioData data;
        Animator animator;
        BoxCollider2D[] cols;

        public override void Enter(AbstractStateController machine) {
            data = FindObjectOfType<AudioData>();
            animator = gameObject.GetComponent<Animator>();
            animator.Play("Opponent Death");
            XPController controller = FindObjectOfType<XPController>();
            controller.increaseXp(500);

            cols = gameObject.GetComponents<BoxCollider2D>();
            SoundManager.PlaySound(data.opDeath);
            foreach (BoxCollider2D col in cols) {
                col.enabled = false;
            }
        }

        public override void HandlePhysics() {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Dead") {
                gameObject.SetActive(false);
            }
        }
    }
    */
    public class OpponentDeathState : AbstractDeathState {
        protected override void Awake() {
            base.Awake();
            _deathAnimation = "Opponent Death";
        }


        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            GetComponent<Rigidbody2D>().isKinematic = true;
            RemoveColliders(_cols);
            Destroy(gameObject, 4f);
        }
    }

}
