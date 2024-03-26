using Assets.Project.Scripts;
using UnityEngine;

namespace Assets.Scripts.Player {
    public class AbstractShootingState : State {
        [SerializeField] protected AbstractBulletManager _manager;
        [SerializeField] protected GameObject _bullet;

        protected Animator _animator;
        protected SpriteRenderer _spriteRenderer;
        protected AbstractCooldownManager _cooldownManager;
        // Инициализируется в реализации
        protected State _nextState;

        // Инициализируется в реализации
        protected float _shotsCount;
        // Инициализируется в реализации
        protected string _cooldownName;

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _cooldownManager = GetComponent<AbstractCooldownManager>();
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _cooldownManager.ResetCooldown(_cooldownName);
        }
    }
}