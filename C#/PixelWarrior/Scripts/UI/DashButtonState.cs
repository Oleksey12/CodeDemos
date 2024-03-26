using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
 * Скрипт высчитывает текущее состояние перезарядки умения "Рывок в долях
 */

public class DashButtonState : MonoBehaviour, ICountAbilityState
{
    private PlayerData playerData;
    private IDashController dashData;

    private void Start()
    {
        playerData = FindObjectOfType<PlayerData>();
        dashData = FindObjectOfType<DashController>();
    }
    public float AbilityState()
    {
        return dashData.DashCooldown / playerData.DashCooldown;
    }
}
