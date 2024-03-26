using Assets.Scripts.Player;
using Assets.Scripts;
using Zenject;
using UnityEngine;
using System;

namespace Assets.Project.Scripts.Player {
    public class EightDirectionMovement : AbstractMovementState {
        [Inject] protected PlayerData _playerData;
        [SerializeField] protected bool debug;
        
        // Компоненты
        protected AbstractCooldownManager _cooldowns;
        protected AbstractMovePredictor _prediction;
        protected State _chargeState;
        protected State _attackState;

        // Направление движения игрока
        protected Vector2 _moveVector;

        // Названия анимаций
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

        // Переменные для смены анимаций    
        protected string _currentAnimation = IDLE;
        protected bool _isStopped = true;
        protected bool _isStopping = false;
        protected bool _isNorth = false;
        protected bool _isWest = false;
        protected float _stopAnimationDuration;

        // Переменные отладки
        private bool _checkRunning = true;
        private float _time = 0f;


        protected override void Awake() {
            base.Awake();
            _speed = _playerData.gameParams.speed.GetValue();
            _stopAnimationDuration = GetStopAnimationDuration(_animator);
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _currentAnimation = IDLE;
            _isStopping = false;
            //_isStopped = true;
        }

        public override void HandlePhysics() {
            _rb.velocity = Vector2.zero;

            _prediction.HandlePhysics(_inputVector, _isStopping);
            _moveVector = _prediction.CalculateMoveDirection();

            MovePosition(_rb, _moveVector, _speed);
            PlayMoveAnimation(_animator, _spriteRenderer);
        }

        public override void HandleLogic() {
            base.HandleLogic();

            if(debug) CountRunDelay(_inputVector, _moveVector, ref _checkRunning, ref _time);

            _prediction.HandleLogic(_inputVector);
        }

        public override void HandleInput() {
            if (_cooldowns.IsCooldownReady(_playerData.chargeSettings.chargeName.GetValue())) {
                if (Input.GetKeyDown(KeyCode.Space)) {
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

        public override void Exit() {
        }

        protected override void PlayMoveAnimation(Animator animator, SpriteRenderer spriteRenderer) {
            ValueTuple<bool, bool> runningState = _prediction.GetRunningState();

            bool isRunningX = runningState.Item1;
            bool isRunningY = runningState.Item2;

            // Состояние бега
            if ((isRunningX || isRunningY) && !_isStopping) {
                if (!ChangeDirectionConditions(_inputVector, _moveVector, _isWest, _isNorth, _isStopped)) {
                    _isStopped = false;
                    _isNorth = _inputVector.y > 0;
                    _isWest = _inputVector.x < 0;

                    if (isRunningX) {
                        spriteRenderer.flipX = _isWest;
                        ChangeAnimationState(!_isNorth ? RUN : RUN_BACK);
                    } else {
                        spriteRenderer.flipX = false;
                        ChangeAnimationState(!_isNorth ? RUN_VERTICAL : RUN_VERTICAL_BACK);
                    }
                    return;
                }
            }

            // Остановка персонажа
            if (!_isStopped && !_isStopping) {
                _isStopping = true;

                if (animator.GetCurrentAnimatorStateInfo(0).IsName(RUN_VERTICAL) || 
                    animator.GetCurrentAnimatorStateInfo(0).IsName(RUN_VERTICAL_BACK)) {
                    ChangeAnimationState(_isNorth ? STOP_VERTICAL_BACK : STOP_VERITCAL);
                } else {
                    ChangeAnimationState(_isNorth ? STOP_BACK : STOP);
                }
                Invoke("stopPlayer", _stopAnimationDuration);
            }

            // Состоянее бездействия
            if (_isStopped) {
                ChangeAnimationState(_isNorth ? IDLE_BACK : IDLE);
            }
        }

        protected void stopPlayer() {
            _isStopped = true;
            _isStopping = false;
        }

        protected bool ChangeDirectionConditions(Vector2 inputVector, 
                                               Vector2 moveVector,
                                               bool isWest, 
                                               bool isNorth, 
                                               bool isStopped) {

            bool changeDirection = true;

            // Проверка условий смены направления на противоположное, 
            // при движении по горизонтали и по диагонали
            changeDirection &= isWest != inputVector.x < 0 && (moveVector.y == 0 || moveVector.x != 0);

            // Условия для смены направления направелния
            // на противоположное при движении по вертикали
            changeDirection |= isNorth != inputVector.y > 0 && moveVector.x == 0;

            // Если игрок до этого стоял на месте,
            // то это не считается сменой направления
            changeDirection &= !isStopped;

            return changeDirection;
        }

        protected override Vector2 GetMoveDirection() {
            float movementInputX = Input.GetAxis("Horizontal");
            float movementInputY = Input.GetAxis("Vertical");
            return new Vector2(movementInputX, movementInputY);
        }

        protected void ChangeAnimationState(string newState) {
            if (_currentAnimation == newState) return;

            _animator.Play(newState);

            _currentAnimation = newState;
        }

        protected float GetStopAnimationDuration(Animator animator) {
            float duration = 0f;

            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips) {
                if (clip.name == STOP)
                    duration = clip.length;
            }
            if (duration == 0f) throw new ArgumentNullException("Error, animation " + STOP + " not found!");

            duration -= Time.fixedDeltaTime / 2;
            return duration;

        }

        protected void CountRunDelay(Vector2 inputVector, 
                                     Vector2 moveVector, 
                                     ref bool checkRunning, 
                                     ref float time) {
            
            if (inputVector == Vector2.zero)
                checkRunning = true;

            if (checkRunning) {
                if (inputVector == Vector2.zero) {
                    time = 0f;
                }
                // Начинаем измеренее, как только пользователь нажмёт одну из кнопок бьега
                if (inputVector != Vector2.zero) {
                    time += Time.deltaTime;
                    // Вычисляем время, через которое персонаж начал бежать
                    if (moveVector != Vector2.zero) {
                        Debug.Log(time);
                        checkRunning = false;
                    }
                }

            }
        }

        protected override void GetAllComponents() {
            base.GetAllComponents();
            _prediction = GetComponent<AbstractMovePredictor>();
            _cooldowns = GetComponent<AbstractCooldownManager>();
            _chargeState = GetComponent<AbstractChargeState>();
            _attackState = GetComponent<AbstractShootingState>();
        }
    }
}
