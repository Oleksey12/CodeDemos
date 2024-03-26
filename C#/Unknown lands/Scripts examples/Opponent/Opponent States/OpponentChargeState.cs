using Assets.Scripts;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts.Opponent.Opponent_States {
    public class OpponentChargeState : AbstractChargeState {
        [Inject] protected OpponentStatsNames _paramNames;
        [SerializeField] protected float waitingStageDuration = 0.5f;
        [SerializeField] protected float _changeStatePause = 0.4f;

        protected OpponentStats _stats;
        protected Animator _animator;
        protected SpriteRenderer _render;

        protected float _waitingStageTimeLeft;
        protected float _chargeDuration;
        protected float _chargeTimeLeft;

        public override void Enter(AbstractStateController machine) {
            _abilityName = _stats.GetValue<string>(_paramNames.paramChargeName);
            _chargeDuration = _stats.GetValue<float>(_paramNames.paramChargeDuration);
            _chargeSpeed = _stats.GetValue<float>(_paramNames.paramChargeSpeed);

            base.Enter(machine);

            _waitingStageTimeLeft = waitingStageDuration;
            _chargeTimeLeft = _chargeDuration;
            _direction = GetChargeDirection();
            _animator.Play(_direction.y <= 0f ? "Charge" : "Charge back");
            _render.flipX = _direction.x < 0;
        }

        protected Vector2 GetChargeDirection() {
            Vector3 botPosition = gameObject.transform.position;
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            return (playerPos - botPosition).normalized;
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            _waitingStageTimeLeft -= Time.deltaTime;
            if (_waitingStageTimeLeft >= 0f)
                return;

            _chargeTimeLeft -= Time.deltaTime;
            if (_chargeTimeLeft >= 0f) {
                ApplyVelocity(_rb, _direction, _chargeSpeed);
                return;
            }

            _changeStatePause -= Time.deltaTime;
            if (_changeStatePause <= 0f)
                _machine.ChangeState(_nextState); 
        }

        protected override void GetAllComponents() {
            base.GetAllComponents();
            _stats = GetComponent<OpponentStats>();
            _nextState = GetComponent<AbstractMovementState>();
            _animator = GetComponent<Animator>();
            _render = GetComponent<SpriteRenderer>();
        }

    }
}
