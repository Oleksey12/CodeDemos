using Assets.Project.Scripts.Abstract.EntityAbstracts;
using Assets.Scripts;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Project.Scripts.Opponent.Opponent_States {
    public class OpponentMovementState : AbstractMovementState {
        [Inject] protected OpponentStatsNames _paramNames;

        protected OpponentStats _stats;
        protected AI _ai;
        protected AbstractCooldownManager _cooldownManager;
        protected State _chargeState;
        protected Transform _playerLocation;
        protected Transform _botPos;

        protected string _directionBool = "direction";

        protected override void Awake() {
            base.Awake();
            _ai.InitializeMaps();
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _speed = _stats.GetValue<float>(_paramNames.paramWalkSpeed);
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            float chargeDistance = _stats.GetValue<float>(_paramNames.paramChargeActivationDistance);
            if (CheckConditions(_playerLocation.position, 
                                _botPos.position, 
                                _cooldownManager,
                                chargeDistance)) {
                _machine.ChangeState(_chargeState);
            }
        }

        public override void Exit() { }

        /*
        protected void OnCollisionEnter2D(Collision2D collision) {
            GameObject entity = collision.gameObject;
            IDamagable damageClass = collision.gameObject.GetComponent<IDamagable>();
            if (entity.tag == "Player" && damageClass != null) {
                damageClass.ApplyDamage(_stats.GetValue<float>(_paramNames.paramDamage));
            }
        }
        */

        protected bool CheckConditions(Vector3 playerPos, Vector3 botPos, AbstractCooldownManager manager, float maxDistance) {
            bool result = true;

            result &= manager.IsCooldownReady(_stats.GetValue<string>(_paramNames.paramChargeName));
            if (!result)
                return false;
            LayerMask mask = LayerMask.GetMask("Player") | LayerMask.GetMask("Walls");

            RaycastHit2D hit = Physics2D.Raycast(botPos,
                                         playerPos - botPos, maxDistance, mask);

            
            result &= (hit.collider != null && hit.collider.gameObject.tag == "Player");
            return result;
        }

        protected override Vector2 GetMoveDirection() {
            return _ai.InterestPathLogic();
        }

        protected override void PlayMoveAnimation(Animator animator, SpriteRenderer spriteRenderer) {
            AnimationScripts.HandleDirection(spriteRenderer, _inputVector.x);
            _animator.SetBool(_directionBool, _inputVector.y > 0);
        }

        protected override void GetAllComponents() {
            base.GetAllComponents();
            _playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
            _botPos = gameObject.transform;

            _stats = GetComponent<OpponentStats>();
            _ai = GetComponent<AI>();
            _cooldownManager = GetComponent<AbstractCooldownManager>();
            _chargeState = GetComponent<AbstractChargeState>();
        }
    }
}
