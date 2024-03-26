using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    public abstract class State : MonoBehaviour {
        protected AbstractStateController _machine;
        protected bool _isInitialized = false;

        public virtual void EnterState(AbstractStateController machine) {
            Enter(machine);
            Initialize();
        }

        public virtual void Enter(AbstractStateController machine) {
            _machine = machine;
        }

        public virtual void HandleInput() {
            if (!_isInitialized) return;
        }

        public virtual void HandleLogic() {
            if (!_isInitialized) return;
        }

        public virtual void HandlePhysics() {
            if (!_isInitialized) return;
        }

        protected virtual void Initialize() => _isInitialized = true;


        public virtual void Exit() {

        }
    }
}
