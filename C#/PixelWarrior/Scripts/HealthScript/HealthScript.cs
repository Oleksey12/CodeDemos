using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/*
 * ������, ����������� ��������� ����
 */

public class HealthScript : IHealthScript
{
    // ������ ������������ �� ����������� ��������
    protected IHealthbar healthBar;

    // �������� ���������, ������������� �� ����������� �� ����
    protected virtual void Awake()
    {
        healthBar = GetComponent<IHealthbar>();
    }


    // ���������� ��������
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


    // �������� ������� �������� �� ��������� �����
    public override void ChangeCurrentHealth(float amount)
    {
        _current_health += amount;
        healthBar.maskUpdate(HealthInPercent());
    }


    // ������� ����������� ��� ������ ��������
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
        // ��������� �� ������ �� ���� ��������
        if (_current_health <= 0)
            Death();
    }
}
