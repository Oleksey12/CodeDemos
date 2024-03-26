namespace Assets.Scripts.Player {
    public class StateController : AbstractStateController {
        public StateController() { }
        public StateController(State startState) {
            Initialize(startState);
        }

    }
}
