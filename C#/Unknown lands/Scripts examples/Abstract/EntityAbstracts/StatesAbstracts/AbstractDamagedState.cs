using Assets.Project.Scripts.Functions;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Analytics;

namespace Assets.Project.Scripts {
    public abstract class AbstractDamagedState : State {
        // Инициализируется в реализации
        protected State _nextState;
        protected Collider2D _objectCollider;
        protected Animator _animator;

        // Инициализируется в реализации
        protected float _invincibilityTime;

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
            _objectCollider = HelperFunctions.GetDamageCollider(gameObject.GetComponents<Collider2D>());
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _objectCollider.isTrigger = false;
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            _invincibilityTime -= Time.deltaTime;
            if (_invincibilityTime <= 0) {
                _machine.ChangeState(_nextState);
            }
        }

        public override void Exit() {
            _objectCollider.isTrigger = true;
        }
    }
}
