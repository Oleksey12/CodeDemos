using Assets.Scripts.NPC.NPCStates.Assets.Scripts;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts {
    public class NPCMainController: MonoBehaviour {
        AbstractStateController stateMachine;

        void Start() {
            State startState = gameObject.GetComponent<WaitForPlayer>();
            stateMachine = new StateController(startState);
        }

        private void Update() {
            stateMachine.currentState.HandleInput();
            stateMachine.currentState.HandleLogic();

        }
        private void FixedUpdate() {
            stateMachine.currentState.HandlePhysics();
        }
    }
}
