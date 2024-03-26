using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Скрипт хранит базовые данные об игроке
 */
public abstract class IPlayerDefaultData : MonoBehaviour
{
    protected const string maxHp = "Max_health";
    protected const string curHp = "Current_health";
    protected const string damage = "Damage";
    protected const string speed = "Speed";
    protected const string attackCooldown = "Attack_cooldown";
    protected const string dashCooldown = "Dash_cooldown";
    protected const string playerKnockback = "Knockback";
    protected const string velocityReduce = "Velocity_reduce";
    protected const string dashPower = "Dash_power";
    protected float _maxHpVal = 80.0f;
    // Хп игрока на момент запуска уровня
    protected float _curHpVal = 80.0f;
    // Урон игрока
    protected float _damageVal = 7.0f;
    // Впемя задержки между ударами (ticks)
    protected float _attackCooldownVal = 30f;
    // Задержка между использованием рывка
    protected float _dashCooldownVal = 120f;
    // Насколько сильно игрок откидывает при ударе
    protected float _playerKnockbackVal = 1f;
    // Сопротивление отдачи игрока
    protected float _velocityReduceVal = 0.1f;
    // Скорость передвижения игрока
    protected float _speedVal = 0.8f;
    // Сила способности "рывок"
    protected float _dashPowerVal = 2.3f;

    public float VelocityReduceAmount
    {
        get => _velocityReduceVal;
        set => _velocityReduceVal = value;
    }
    public float PlayerKnockback
    {
        get => _playerKnockbackVal;
        set => _playerKnockbackVal = value;
    }
    public float Speed
    {
        get => _speedVal;
        set => _speedVal = value;
    }

    public float AttackCooldown
    {
        get => _attackCooldownVal;
        set => _attackCooldownVal = value;
    }

    public float DashCooldown
    {
        get => _dashCooldownVal;
        set => _dashCooldownVal = value;
    }
    public float Damage
    {
        get => _damageVal;
        set => _damageVal = value;
    }

    public float CurrentHealth
    {
        get => _curHpVal;
        set => _curHpVal = value;
    }

    public float MaxHealth
    {
        get => _maxHpVal;
        set => _maxHpVal = value;
    }

    public float DashPower
    {
        get => _dashPowerVal;
        set => _dashPowerVal = value;
    }
}

