using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

/*
 * ������ ����������� ��������� ������
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
        // ����������� ���������� � ������� ������
        playerData = GetComponent<IPlayerDefaultData>();
        // ��������� �� ������ � ������������ � ��������� �������
        setHealth(playerData.CurrentHealth, playerData.MaxHealth);
        // ���������� ������� �������� ������
        healthBar.maskUpdate(HealthInPercent());
    }

}
