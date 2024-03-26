using Assets.Scripts;
using UnityEngine;

namespace Assets.Project.Scripts {
    public abstract class AbstractChargeState : State {
        protected AbstractCooldownManager _cooldownManager;
        protected Rigidbody2D _rb;

        // Инициализируется в реализации
        protected State _nextState;

        // Инициализируется в реализации
        protected string _abilityName;

        // Инициализируется в реализации
        protected float _chargeSpeed;

        // Инициализируется в реализации
        protected Vector2 _direction;

        protected virtual void Awake() {
            GetAllComponents();
        }
        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _cooldownManager.ResetCooldown(_abilityName);

        }
        protected virtual void ChangeState(State state) {
            _machine.ChangeState(state);
        }
        protected virtual void ApplyVelocity(Rigidbody2D rb, Vector2 direction, float chargeSpeed) {
            _rb.MovePosition(_rb.position + direction * chargeSpeed * Time.deltaTime);
            //_rb.velocity = direction * chargeSpeed;
        }
        protected virtual void GetAllComponents() {
            _cooldownManager = GetComponent<AbstractCooldownManager>();
            _rb = GetComponent<Rigidbody2D>();
        }
    }
}
