using Assets.Project.Scripts;
using Assets.Project.Scripts.Abstract.EntityAbstracts;
using Assets.Scripts;
using Zenject;
using UnityEngine;

public class PlayerMainController : AbstractMainController, IPausable, IDamagable {
    [Inject] protected PlayerData _data;

    protected State _damaged;
    protected State _pause;
    protected State _death;

    protected override void Awake()
    {
        base.Awake();
        _death = GetComponent<AbstractDeathState>();
        _damaged = GetComponent<AbstractDamagedState>();
        _pause = GetComponent<AbstractPausedState>();   
    }

    protected override void SetStartState() {
        State startState = GetComponent<AbstractMovementState>();
        _stateMachine.Initialize(startState);
    }

    public void ApplyDamage(float damage) {
        _data.gameParams.currentHealth.IncreaseValue(-(int)damage);
        if(_data.gameParams.currentHealth.GetValue() <= 0) {
            _stateMachine.ChangeState(_death);
        }
        else {
            _stateMachine.ChangeState(_damaged);
        }
    }

    public void Pause() {
        _stateMachine.ChangeState(_pause);
    }
}
