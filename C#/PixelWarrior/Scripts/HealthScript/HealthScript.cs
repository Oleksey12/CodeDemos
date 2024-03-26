using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Скрипт, управляющий здоровьем бота
 */

public class HealthScript : IHealthScript
{
    // Скрипт ответсвенный за отображение здоровья
    protected IHealthbar healthBar;

    // Хешируем компонент, ответственный за отображение хп бота
    protected virtual void Awake()
    {
        healthBar = GetComponent<IHealthbar>();
    }


    // Установить здоровье
    public override void setHealth(float curHP,float maxHP)
    {
        if (maxHP <= 0)
        {
            Debug.Log("Invalid health " + maxHP);
            return;
        }
        _max_health = maxHP;

        if (curHP > _max_health)
            _current_health = _max_health;
        else
            _current_health = curHP;
        healthBar.maskUpdate(HealthInPercent());
    }


    // Изменить текущее здоровье на некоторое число
    public override void ChangeCurrentHealth(float amount)
    {
        _current_health += amount;
        healthBar.maskUpdate(HealthInPercent());
    }


    // Функция вызвываемая при смерти существа
    protected override void Death()
    {
        healthBar.DestroyHealthbar();
        Destroy(gameObject);
        return;
    }

    protected float HealthInPercent()
        { return _current_health / _max_health; }
    protected private void FixedUpdate()
    {
        // Проверяем не умерло ли наше существо
        if (_current_health <= 0)
            Death();
    }
}
