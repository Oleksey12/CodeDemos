using Assets.Scripts;
using UnityEngine;

namespace Assets.Project.Scripts {
    public class AbstractDeathState : State {
        protected Animator _animator;
        protected BoxCollider2D[] _cols;
        // Инициализируется в реализации
        protected string _deathAnimation;

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
            _cols = GetComponents<BoxCollider2D>();
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _animator.Play(_deathAnimation);
        }

        protected virtual void RemoveColliders(BoxCollider2D[] cols) {
            foreach (BoxCollider2D col in cols)
                col.enabled = false;
        }

    }

}
