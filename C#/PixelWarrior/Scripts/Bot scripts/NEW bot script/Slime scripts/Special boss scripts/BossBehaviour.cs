using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
 * Скрипт реализует поведение босса
 */
public class BossBehaviour : SlimeBotBehaviour
{
    Animator _animator;
    IAbilityActivator _activator;
    protected int _cooldownState = 0;
    [SerializeField] int _cooldownDelay= 200;
    protected override void Start()
    {
        base.Start();
        _cooldownState = _cooldownDelay;
    }

    protected override void HashComponents()
    {
        base.HashComponents();
        _activator = GetComponent<IAbilityActivator>();
        _animator = GetComponent<Animator>();
    }


    public override void Behave()
    {
        if (_cooldownState == 0)
        {
            _cooldownState = _cooldownDelay;
            _activator.ActivateCurrentAbility();
        }
        if (!_activator.GetAbilityState())
        {

            _animator.SetBool("IsCasting", false);
            _followPlayer.ChasePlayer();
            --_cooldownState;
            
            _botMoveAnimation.SetMoveDirection();
            

        }
        else
        {
            AbilityDirection(_targetObj);
            _animator.SetBool("IsCasting", true);
        }
            
    }

    public void AbilityDirection(GameObject targetObj)
    {
        // Во время активации способности бот смотрит в сторону игрока
        if (targetObj.transform.position.x > gameObject.transform.position.x)
            gameObject.transform.localScale = new Vector3(-_characteristics.Size, _characteristics.Size, _characteristics.Size);
        else
            gameObject.transform.localScale = new Vector3(_characteristics.Size, _characteristics.Size, _characteristics.Size);

    }

}

