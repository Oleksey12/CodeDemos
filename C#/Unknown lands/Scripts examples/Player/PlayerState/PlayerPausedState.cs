using Assets.Scripts;


namespace Assets.Project.Scripts {
    public class PlayerPausedState : AbstractPausedState {
        protected override void Awake() {
            base.Awake();
            _nextState = GetComponent<AbstractMovementState>();
        }

        protected override void Unpause() {
            _machine.ChangeState(_nextState);
        }
    }
}
