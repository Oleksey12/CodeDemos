using Assets.Scripts;


namespace Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts {
    public abstract class AbstractWaitingState : State {
        // Инициализируется в реализации
        protected State _nextState;

        public override void HandlePhysics() {
            if (Condition()) {
                _machine.ChangeState(_nextState);
            }
        }
        
        protected abstract bool Condition();
    }
}
