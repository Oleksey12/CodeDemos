using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ������ ���������� ��������� ������
 */
public class ButtonCooldownEffect : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;

    // ��� ��������� ������ � ��������� ������
    ICountAbilityState abilityState;

    void Start()
    {
        abilityState = GetComponent<ICountAbilityState>();
    }

    private void FixedUpdate()
    {
        cooldownImage.fillAmount = abilityState.AbilityState();
    }

}
