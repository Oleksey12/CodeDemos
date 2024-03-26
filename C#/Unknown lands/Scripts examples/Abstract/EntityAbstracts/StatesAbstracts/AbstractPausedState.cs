using Assets.Scripts;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts {
    public abstract class AbstractPausedState : State {
        [Inject] protected IPauseManager _manager;
        // Инициализируется в реализации
        protected State _nextState;
        protected Collider2D[] _colliders;

        protected virtual void Awake() {
            _colliders = GetComponents<Collider2D>();
        }
        public override void HandlePhysics() {
            base.HandlePhysics();
            if (!_manager.IsPaused()) {
                Unpause();
            }
        }
        protected abstract void Unpause();
        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            foreach (var collider in _colliders) {
                collider.enabled = false;
            }
        }
        public override void Exit() {
            foreach (var collider in _colliders) {
                collider.enabled = true;
            }
        }

    }
}
