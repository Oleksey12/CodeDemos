using Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Project.Scripts.Opponent.Opponent_States {
    public class OpponentWaitingState : AbstractWaitingState {
        [SerializeField] protected float triggerDistance = 1f;
        //Collider2D[] _colliders;
        protected Animator _animator;
        protected Transform _playerPos;
        protected Transform _botPosition;

        protected bool _isTriggered = false;

        protected virtual void Awake() {
            GetAllComponents();
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _playerPos = GameObject.FindWithTag("Player").transform;
            _botPosition = gameObject.transform;
            //HelperFunctions.GetDamageCollider(_colliders).enabled = false;
        }

        protected override bool Condition() {
            if (!_isTriggered) {
                if ((_botPosition.position - _playerPos.position).magnitude <= triggerDistance) {
                    _isTriggered = true;
                    _animator.SetBool("active", _isTriggered);
                }
                return false;
            }
            return _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "StandUp";
        }

        private void GetAllComponents() {
            //_colliders = GetComponent<Collider2D>();
            _animator = gameObject.GetComponent<Animator>();
            _nextState = gameObject.GetComponent<AbstractMovementState>();
        }
    }
}
