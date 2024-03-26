using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

/*
 * —крипт управл€ющий здоровьем игрока
 */
public class PlayerHealthScript : HealthScript
{
    IPlayerDefaultData playerData;

    
    protected override void Death()
    {
        Destroy(gameObject);
        Debug.Log("GG, you died");
        SceneManager.LoadScene("NewMainMenu");
    }

    protected void Start()
    {
        // ’еширование переменной с данными игрока
        playerData = GetComponent<IPlayerDefaultData>();
        // ”становка хп игрока в соответствии с хранимыми данными
        setHealth(playerData.CurrentHealth, playerData.MaxHealth);
        // ќтображаем текущее здоровье игрока
        healthBar.maskUpdate(HealthInPercent());
    }

}
