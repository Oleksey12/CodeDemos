using UnityEngine;

namespace Assets.Scripts {
    public abstract class AbstractStateController {
        public State currentState { get; protected set; }
        public virtual void Initialize(State startState) {
            currentState = startState;
            currentState.EnterState(this);
        }

        public virtual void ChangeState(State newState) {
            currentState.Exit();
            currentState = newState;
            currentState.EnterState(this);
        }
    }
}
