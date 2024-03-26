using UnityEngine;

namespace Assets.Scripts.NPC.NPCStates {
    using UnityEngine;

    namespace Assets.Scripts {
        public class WaitForPlayer : State {
            [SerializeField] Transform player;
            PlayerDataController playerStats;
            Animator animator;
            NPCStats stats;
            State appearState;
            Transform NPCPosition;
            AbstractStateController stateController;


            public override void Enter(AbstractStateController machine) {
                animator = GetComponent<Animator>();
                stats = gameObject.GetComponent<NPCStats>();
                appearState = gameObject.GetComponent<ReadyForDialog>();
                stateController = machine;
                NPCPosition = gameObject.transform;
            }

            /*
            public override void HandlePhysics() {
                if (playerStats.currentLevel == 10) {
                    stateController.ChangeState(appearState);
                    return;
                }
            }
            */
            public override void Exit() {
                Debug.Log("found!");
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                animator.Play("Appear");

            }
        }

    }
}
