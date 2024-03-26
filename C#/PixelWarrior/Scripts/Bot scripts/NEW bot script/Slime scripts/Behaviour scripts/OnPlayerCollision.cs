using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
 * Скрипт управляет поведением бота при столкновении с игроком
 */
public class OnPlayerCollision : MonoBehaviour, IOnPlayerCollision
{
    IBotCharacteristics _characteristics;
    IKnockbackController _botKnockbackController;
    IKnockbackController _playerKnockbackController;
    private void Start()
    {
        HashComponents();
    }

    public void RegisterCollision(GameObject collisionObject)
    {
        IHealthScript playerHealth = collisionObject.GetComponent<IHealthScript>();
        playerHealth.ChangeCurrentHealth(-_characteristics.Damage);

        _playerKnockbackController = collisionObject.GetComponent<IKnockbackController>();  
        ApplyKnockback(collisionObject.transform.position,gameObject.transform.position);
        

    }

    private void ApplyKnockback(Vector3 playerObj, Vector3 botObj)
    {
        // Игрок находится справа от бота
        if(playerObj.x > botObj.x)
        {
            _botKnockbackController.ApplyKnockback(new Vector2(-_characteristics.InputKnockBack, 0));
            _playerKnockbackController.ApplyKnockback(new Vector2(_characteristics.Knockback, 0));
        }
        else // Игрок находится слева от бота
        {
            _botKnockbackController.ApplyKnockback(new Vector2(_characteristics.InputKnockBack, 0));
            _playerKnockbackController.ApplyKnockback(new Vector2(-_characteristics.Knockback, 0));
        }
    }

    private void HashComponents()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        //_playerKnockbackController = FindObjectOfType<KnockbackController>();
        _botKnockbackController = GetComponent<IKnockbackController>();
    }
}

