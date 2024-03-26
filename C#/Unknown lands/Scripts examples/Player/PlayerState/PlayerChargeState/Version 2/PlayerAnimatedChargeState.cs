using Assets.Project.Scripts.Opponent;
using Assets.Scripts;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts.Player {
    public class PlayerAnimatedChargeState : AbstractChargeState {
        [Inject] protected PlayerData _data;
        [SerializeField] protected float moveDuration = 0.8f;

        protected Animator _animator;
        protected SpriteRenderer _render;

        protected float _moveTimeLeft;
        protected float _waitingStageTimeLeft;
        protected float _chargeDuration;

        protected const string CHARGE = "Charge";
        protected const string CHARGE_BACK = "Charge back";
        protected const string CHARGE_VERTICAL= "Charge vertical";
        protected const string CHARGE_VERTICAL_BACK = "Charge vertical back";

        public override void Enter(AbstractStateController machine) {
            _abilityName = _data.chargeSettings.chargeName.GetValue();

            base.Enter(machine);

            _moveTimeLeft = moveDuration;
            _chargeDuration = _data.chargeSettings.chargeTime.GetValue();
            _chargeSpeed = _data.chargeSettings.chargeSpeed.GetValue();

            StartCharge(ref _direction, _animator, _render);
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            _chargeDuration -= Time.deltaTime;
            _moveTimeLeft -= Time.deltaTime;

            if (_moveTimeLeft > 0)
                ApplyVelocity(_rb, _direction.normalized, _chargeSpeed);

            if (_chargeDuration <= 0f)
                _machine.ChangeState(_nextState);
        }

        protected void StartCharge(ref Vector2 direction, Animator animator, SpriteRenderer renderer) {
            direction = GetChargeDirection();

            if (direction == Vector2.zero) {
                direction.x = _render.flipX ? -1 : 1;
                direction.y = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "IDLE" ? 0.00001f : -0.00001f;
            }

            HandleAnimation(animator, renderer);
        }

        private void HandleAnimation(Animator animator, SpriteRenderer renderer) {
            if (_direction.x == 0) {
                animator.Play(_direction.y <= 0f ? CHARGE_VERTICAL_BACK : CHARGE_VERTICAL);
            } else {
                animator.Play(_direction.y <= 0f ? CHARGE_BACK : CHARGE);
            }
     
            //renderer.flipX = _direction.x < 0;
        }

        protected virtual Vector2 GetChargeDirection() {
            float movementInputX = Input.GetAxis("Horizontal");
            float movementInputY = Input.GetAxis("Vertical");
            return new Vector2(movementInputX, movementInputY);
        }

        protected override void GetAllComponents() {
            base.GetAllComponents();
            _nextState = GetComponent<AbstractMovementState>();
            _animator = GetComponent<Animator>();
            _render = GetComponent<SpriteRenderer>();
        }

    }
}
