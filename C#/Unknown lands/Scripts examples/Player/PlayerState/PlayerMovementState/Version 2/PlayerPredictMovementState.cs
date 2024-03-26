using Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts;
using Assets.Scripts;
using Assets.Scripts.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts {
    public class PlayerPredictMovementState : AbstractMovementState {
        [Inject] protected PlayerData _playerData;

        protected AbstractCooldownManager _cooldowns;
        protected State _chargeState;
        protected State _attackState;
        protected OldMovePredictor _prediction;

        protected string _directionAnimationName;
        protected Vector2 _moveVector;

        //protected int animationTicks = 0;

        protected override void Awake() {
            base.Awake();
            _moveAnimationName = "IsRunning";
            _directionAnimationName = "Direction";
            _speed = _playerData.gameParams.speed.GetValue();
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            _rb.velocity = Vector2.zero;

            _moveVector = _prediction.CalculateMoveDirection(_inputVector);

            MovePosition(_rb, _moveVector, _speed);
            PlayMoveAnimation(_animator, _spriteRenderer);
        }
        public override void HandleLogic() {
            base.HandleLogic();
            _prediction.UpdateInputs(_inputVector);
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
            AnimationScripts.HandleDirection(spriteRenderer, _moveVector.x);

            List<bool> runningLog = _prediction.GetRunningLog();
            Vector2 moveCriterias = _prediction.GetMoveCriterias();

            var last = runningLog.Take((int)moveCriterias.x);
            bool stopRunning = runningLog.All(o => o == false);
            bool startRunning = last.All(o => o == true);

            if (startRunning) {
                AnimationScripts.SetAnimatorBoolean(animator, _directionAnimationName, _moveVector.y <= 0f);
            }

            if (runningLog.Count == (int)moveCriterias.y && (stopRunning || startRunning)) {
                AnimationScripts.SetAnimatorBoolean(animator, _moveAnimationName, runningLog[0]);  
            }
            
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
