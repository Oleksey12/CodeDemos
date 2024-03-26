using UnityEngine;

namespace Assets.Scripts.NPC.NPCStates {
    public class ReadyForDialog : State {
        [SerializeField] Transform player;
        Animator animator;
        NPCStats stats;
        State DisappearState;
        Transform NPCPosition;
        AbstractStateController stateController;


        public override void Enter(AbstractStateController machine) {
            DisappearState = gameObject.GetComponent<Dissapear>();
            animator = GetComponent<Animator>();
            stats = gameObject.GetComponent<NPCStats>();
            stateController = machine;
            NPCPosition = gameObject.transform;
        }

        public override void HandleInput() {
            if ((NPCPosition.localPosition - player.localPosition).magnitude < stats.dialogRadius) {
                if (Input.GetMouseButtonDown(1)) {
                    Debug.Log("Здарова, карта!");
                    stateController.ChangeState(DisappearState);
                }
            }
        }

        public override void Exit() {
            animator.Play("Disappear");
        }
    }
}
