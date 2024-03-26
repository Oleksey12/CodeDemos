using Assets.Scripts.Player;
using Assets.Scripts;
using Zenject;
using System.Collections.Generic;
using System.Linq;
using static GameParams;
using UnityEngine;


namespace Assets.Project.Scripts.Player.PlayerState.PlayerMovementState.Version_2 {
    public class OldEightDirectionMovement : AbstractMovementState {
        [Inject] protected PlayerData _playerData;

        protected AbstractCooldownManager _cooldowns;
        protected State _chargeState;
        protected State _attackState;
        protected OldMovePredictor _prediction;

        protected string _directionAnimationName;
        protected Vector2 _moveVector;

        //protected int animationTicks = 0;

        protected bool stopRunning = true;
        protected bool startRunning = false;


        protected bool isStopped = true;
        protected bool isStopping = false;
        protected bool isNorth = false;
        protected bool isWest = false;

        protected const string IDLE = "IDLE";
        protected const string IDLE_BACK = "IDLE back";
        protected const string RUN = "Run";
        protected const string RUN_BACK = "Run back";
        protected const string RUN_VERTICAL = "Run vertical";
        protected const string RUN_VERTICAL_BACK = "Run vertical back";
        protected const string STOP = "Stop";
        protected const string STOP_BACK= "Stop back";   
        protected const string STOP_VERITCAL = "Stop vertical";
        protected const string STOP_VERTICAL_BACK= "Stop vertical back";


        protected string currentAnimation = IDLE;

        protected override void Awake() {
            base.Awake();
            _moveAnimationName = "IsRunning";
            _directionAnimationName = "Direction";
            _speed = _playerData.gameParams.speed.GetValue();
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            _rb.velocity = Vector2.zero;

            _prediction.FixedUpdateInputs(_inputVector, isStopping);
            _moveVector = _prediction.CalculateMoveDirection(_inputVector);

            MovePosition(_rb, _moveVector, _speed);

            PlayMoveAnimation(_animator, _spriteRenderer);
        }
        public override void HandleLogic() {
            base.HandleLogic();
            _prediction.UpdateInputs(_inputVector);
        }


        protected void ChangeAnimationState(string newState) {
            if (currentAnimation == newState) return;

            _animator.Play(newState);

            currentAnimation = newState;
        }


        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            isStopping = false;
        }


        public override void HandleInput() {
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
            List<bool> runningLog = _prediction.GetRunningLog();
            Vector2 moveCriterias = _prediction.GetMoveCriterias();

            var last = runningLog.Take((int)moveCriterias.x);
            stopRunning = runningLog.All(o => o == false);
            startRunning = last.All(o => o == true);

            if (isStopping)
                return;

            // Если персонаж бежит
            if (startRunning || (!isStopped && !stopRunning)) {
                isStopped = false;
                if (_moveVector.x != 0f) {
                    isNorth = _moveVector.y > 0;
                    AnimationScripts.HandleDirection(spriteRenderer, _moveVector.x);
                    ChangeAnimationState(_moveVector.y <= 0 ? RUN : RUN_BACK);
                } else if (_moveVector.y != 0f) {
                    isNorth = _moveVector.y > 0;
                    spriteRenderer.flipX = false;
                    ChangeAnimationState(_moveVector.y <= 0 ? RUN_VERTICAL : RUN_VERTICAL_BACK);
                }
                return;
            }

            // Остановка персонажа
            if (!isStopped) {
                isStopping = true;
                //AnimationScripts.HandleDirection(spriteRenderer, _inputVector.x);

                if (_inputVector.x == 0) {
                    ChangeAnimationState(isNorth ? STOP_VERTICAL_BACK : STOP_VERITCAL);
                } else {
                    ChangeAnimationState(isNorth ? STOP_BACK : STOP);
                }
                float delay = animator.GetCurrentAnimatorStateInfo(0).length - Time.deltaTime * 2;
                Invoke("stopPlayer", delay);
            }

            // При полной остановке
            if (isStopped) {
                ChangeAnimationState(isNorth ? IDLE_BACK : IDLE);
            }
        }

        protected void stopPlayer() {
            isStopped = !startRunning;
            isStopping = false;
        }


        protected override Vector2 GetMoveDirection() {
            float movementInputX = Input.GetAxis("Horizontal");
            float movementInputY = Input.GetAxis("Vertical");
            return new Vector2(movementInputX, movementInputY);
        }

      
        protected override void GetAllComponents() {
            base.GetAllComponents();
            _cooldowns = GetComponent<AbstractCooldownManager>();
            _chargeState = GetComponent<AbstractChargeState>();
            _attackState = GetComponent<AbstractShootingState>();
            _prediction = GetComponent<OldMovePredictor>();
        }
    }
}
