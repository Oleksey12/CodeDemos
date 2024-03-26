using Assets.Project.Scripts;
using Assets.Project.Scripts.Abstract.EntityAbstracts;
using Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts;
using Assets.Project.Scripts.Opponent;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Opponent {

    public class OpponentMainController : AbstractMainController, IPausable, IDamagable {
        [Inject] protected OpponentStatsNames _paramNames;

        protected OpponentStats _stats;
        protected State _damaged;
        protected State _pause;
        protected State _death;

        protected override void Awake() {
            base.Awake();
            GetAllComponents();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            //Debug.Log(_stateMachine.GetHashCode());
        }

        protected override void SetStartState() {
            State startState = gameObject.GetComponent<AbstractWaitingState>();
            _stateMachine.Initialize(startState);
        }

        public void Pause() {
            _stateMachine.ChangeState(_pause);
        }

        public void ApplyDamage(float damage) {
            _stats.IncreaseFloatValue(_paramNames.paramCurrentHealth, -damage);
            if (_stats.GetValue<float>(_paramNames.paramCurrentHealth) <= 0) {
                _stateMachine.ChangeState(_death);
            } else {
                _stateMachine.ChangeState(_damaged);
            }
        }

        protected void GetAllComponents() {
            _stats = gameObject.GetComponent<OpponentStats>();
            _damaged = gameObject.GetComponent<AbstractDamagedState>();
            _pause = gameObject.GetComponent<AbstractPausedState>();
            _death = gameObject.GetComponent<AbstractDeathState>();
        }
    }

}
