using Assets.Scripts;
using Assets.Scripts.Player;
using UnityEngine;

public abstract class AbstractMainController : MonoBehaviour {
    protected AbstractStateController _stateMachine;

    protected virtual void Awake() {
        // Нарушение принципов DI 💀
        _stateMachine = new StateController();
    }
    protected virtual void Start() {
        SetStartState();
    }
    protected abstract void SetStartState();

    protected virtual void Update() {
        _stateMachine.currentState.HandleInput();
        _stateMachine.currentState.HandleLogic();
    }
    protected virtual void FixedUpdate() {
        _stateMachine.currentState.HandlePhysics();
    }
}