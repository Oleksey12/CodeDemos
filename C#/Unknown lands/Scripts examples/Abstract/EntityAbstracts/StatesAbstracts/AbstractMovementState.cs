using UnityEngine;

namespace Assets.Scripts {
    public abstract class AbstractMovementState : State {
        //protected AudioSource soundManager;
        protected Animator _animator;
        protected Rigidbody2D _rb;
        protected SpriteRenderer _spriteRenderer;

        // Инициализируется в реализации
        protected float _speed;
        // Инициализируется в реализации
        protected Vector2 _inputVector;
        // Инициализируется в реализации
        protected string _moveAnimationName;

        protected virtual void Awake() {
            GetAllComponents();
        }

        public override void HandleLogic() {
            base.HandlePhysics();
            _inputVector = GetMoveDirection();
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _inputVector = Vector2.zero;
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            MovePosition(_rb, _inputVector, _speed);
        }

        public override void Exit() {
            AnimationScripts.SetAnimatorBoolean(_animator, _moveAnimationName, false);
        }

        protected virtual Vector2 GetMoveDirection() {
            return new Vector2();
        }

        protected virtual void PlayMoveAnimation(Animator animator, SpriteRenderer spriteRenderer) {
            AnimationScripts.HandleDirection(spriteRenderer, _inputVector.x);
            // Player animation
            AnimationScripts.SetAnimatorBoolean(_animator, _moveAnimationName, _inputVector.magnitude > 0);

        }

        protected virtual void MovePosition(Rigidbody2D rb, Vector2 moveVector, float speed) {
            rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);
        }

        protected virtual void GetAllComponents() {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
        }
    }
}