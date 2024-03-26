using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Скрипт отображает состояние умений
 */
public class ButtonCooldownEffect : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;

    // Для получения данных о состоянии умений
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
