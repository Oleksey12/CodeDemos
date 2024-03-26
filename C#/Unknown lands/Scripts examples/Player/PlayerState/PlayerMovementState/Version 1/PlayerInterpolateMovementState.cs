using Assets.Project.Scripts;
using Assets.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Assets.Scripts {
    /*
    public class MovementState : MainState {
        AudioSource steps;
        Animator animator;
        AbstractStateController stateMachine;
        Rigidbody2D rb;
        SpriteRenderer spriteRenderer;
        PlayerDataController playerData;
        CooldownManager cooldowns;

        State chargeState;
        State attackState;


        Vector2 velocity;
        float lowPressStrength = 0.12f;
        float normalPressStrength = 0.45f;

        float ticksBeforeChange = 2;
        float ticksLeft = 0;
        Vector3 functionKoefs;
        private void Awake() {
            GetAllComponents();
        }
        public override void Enter(AbstractStateController machine) {
            stateMachine = machine;
            velocity = Vector2.zero;
            functionKoefs = MyMath.CalculateKoefficients(lowPressStrength, normalPressStrength);
        }

        public override void HandleInput() {
            if (playerData.freezeImputs) {
                velocity = Vector2.zero;
                return;
            }

            if (cooldowns.IsChargeReady()) {
                if (Input.GetKeyDown(KeyCode.R)) {
                    stateMachine.ChangeState(chargeState);
                    cooldowns.ResetCharge();
                    return;
                }
            }

            if (cooldowns.IsAttackReady()) {
                if (Input.GetMouseButtonDown(0)) {
                    stateMachine.ChangeState(attackState);
                    cooldowns.ResetAttack();
                    return;
                }
            }

            velocity = CountPlayerVelocity();
        }
        public override void HandleLogic() {
            // Уменьшить перезарядку другой способности
            cooldowns.DecreaseCharge(Time.deltaTime);
            cooldowns.DecreaseAttack(Time.deltaTime);
        }
        private void PlayMoveAnimation(Animator animator, Vector2 velocity) {
            if (velocity.y > 0) {
                animator.SetBool("Direction", false);
            } else if (velocity.y <= 0) {
                animator.SetBool("Direction", true);
            }


            if (velocity.magnitude > 0) {
                if (!steps.isPlaying)
                    steps.Play();
                animator.SetBool("IsRunning", true);
            } else {
                steps.Stop();
                animator.SetBool("IsRunning", false);
            }
        }

        public override void HandlePhysics() {
            rb.MovePosition(rb.position + velocity * Time.deltaTime);

            // Вызывать анимации
            ticksLeft -= 1;
            if (ticksLeft < 0) {
                ticksLeft = ticksBeforeChange;
                PlayMoveAnimation(animator, velocity);
                AnimationScripts.HandleDirection(spriteRenderer, velocity.x);
            }
        }

        public override void Exit() {
            animator.SetBool("IsRunning", false);
        }
        private Vector2 CountPlayerVelocity() {
            float movementInputX = Input.GetAxis("Horizontal");
            float movementInputY = Input.GetAxis("Vertical");

            float horizontalSpeed = MyMath.ParabolaFunc(
                movementInputX,
                new Vector2(lowPressStrength, normalPressStrength),
                functionKoefs);

            float verticalSpeed = MyMath.ParabolaFunc
                (movementInputY,
                new Vector2(lowPressStrength, normalPressStrength),
                functionKoefs);


            horizontalSpeed = Mathf.Sign(movementInputX) * horizontalSpeed;
            verticalSpeed = Mathf.Sign(movementInputY) * verticalSpeed;

            return new Vector2(horizontalSpeed, verticalSpeed).normalized * playerData.speed;
        }
        private void GetAllComponents() {
            steps = GetComponent<AudioSource>();
            cooldowns = GetComponent<CooldownManager>();
            chargeState = GetComponent<ChargeState>();
            attackState = GetComponent<AttackState>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerData = GetComponent<PlayerDataController>();
        }
    }
    */
    public class PlayerInterpolateMovementState : AbstractMovementState {
        [Inject]
        protected PlayerData _playerData;

        protected IMoveInterpolation _interpolation;
        protected AbstractCooldownManager _cooldowns;
        protected State _chargeState;
        protected State _attackState;

        protected string _directionAnimationName;

        protected override void Awake() {
            base.Awake();
            _moveAnimationName = "IsRunning";
            _directionAnimationName = "Direction";
            _speed = _playerData.gameParams.speed.GetValue();
        }

        public override void HandleInput() {
            base.HandleInput();
            if (_cooldowns.IsCooldownReady(_playerData.chargeSettings.chargeName.GetValue())) {
                if (Input.GetKeyDown(KeyCode.R)) {
                    _machine.ChangeState(_chargeState);
                    return;
                }
            }
            if (_cooldowns.IsCooldownReady(_playerData.attackSettings.attackName.GetValue())) {
                if (Input.GetMouseButtonDown(0)) {
                    _machine.ChangeState(_attackState);
                    return;
                }
            }
        }

        protected override void PlayMoveAnimation(Animator animator, SpriteRenderer spriteRenderer) {
            base.PlayMoveAnimation(animator, spriteRenderer);
            AnimationScripts.SetAnimatorBoolean(animator, _directionAnimationName, _inputVector.y <= 0);
        }

        protected override Vector2 GetMoveDirection() {
            float movementInputX = Input.GetAxis("Horizontal");
            float movementInputY = Input.GetAxis("Vertical");
            return _interpolation.AdjustPlayerVelocity(movementInputX, movementInputY);
        }

        protected override void GetAllComponents() {
            base.GetAllComponents();
            _interpolation = GetComponent<IMoveInterpolation>();
            _cooldowns = GetComponent<AbstractCooldownManager>();
            _chargeState = GetComponent<AbstractChargeState>();
            _attackState = GetComponent<AbstractShootingState>();
        }
    }
}