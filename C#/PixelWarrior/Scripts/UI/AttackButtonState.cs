using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Скрипт высчитывает текущее состояние перезарядки умения "Атака" в долях
 */
public class AttackButtonState : MonoBehaviour, ICountAbilityState
{
    private IPlayerDefaultData playerData;
    private IAttackController attackData;

    private void Start()
    {
        playerData = FindObjectOfType<IPlayerDefaultData>();
        attackData = FindObjectOfType<AttackController>();
    }
    public float AbilityState()
    {
        return attackData.AttackCooldown / playerData.AttackCooldown;
    }
}

