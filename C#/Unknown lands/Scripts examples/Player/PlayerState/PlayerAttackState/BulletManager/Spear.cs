using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts {
    public class Spear : AbstractBullet {
        [SerializeField] protected GameObject visualPart;
        [SerializeField] protected GameObject physiclalPart;

        protected PlayerData _data;
        protected Animator _animator;
        protected SpriteRenderer _render;

        protected string _spearAnimationName;
        protected bool initialized = false;

        protected override void Awake() {
            base.Awake();
            _damage = _data.attackSettings.attackDamage.GetValue();
            _speed = _data.attackSettings.attackSpeed.GetValue();

            _render = visualPart.GetComponent<SpriteRenderer>();
            _animator = visualPart.GetComponent<Animator>();

            _rb = physiclalPart.GetComponent<Rigidbody2D>();
            _collider = physiclalPart.GetComponent<Collider2D>();

            _spearAnimationName = "hit";
        }

        public override void Initialize(Action bulletCallback,
                                        Vector2 direction,
                                        List<string> damagedTags = null,
                                        List<string> ignoredTags = null,
                                        GameObject parent = null) {

            base.Initialize(bulletCallback, direction, damagedTags, ignoredTags, parent);
            AnimateSpear(_animator, _spearAnimationName);
            AnimationScripts.HandleDirection(_render, direction.x);
            initialized = true;
        }

        protected void FixedUpdate() {
            if (!initialized)
                return;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_spearAnimationName)) {
                Move(_rb, _direction, _speed);
                CheckCollisions(_collider, _damagedObjects, _damage);
            } else {
                Disappear(gameObject);
            }
        }

        protected void AnimateSpear(Animator animator, string animationName) {
            animator.Play(animationName);
        }


        [Inject]
        public void Construct(PlayerData data) {
            _data = data;
        }
    }
}
