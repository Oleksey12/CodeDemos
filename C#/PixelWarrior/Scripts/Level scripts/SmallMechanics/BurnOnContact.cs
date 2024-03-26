using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Скрипт управляет взаимодействием игрока с костром
 */
public class BurnOnContact : MonoBehaviour
{
    private float _fireDamage = 1f;
    private GameObject BurningObject;


    private void ApplyBurnDamage() // Наносит урон игроку при контакте с огнём
    {
        BurningObject.GetComponent<PlayerHealthScript>().ChangeCurrentHealth(-_fireDamage);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            BurningObject = collision.gameObject;
            InvokeRepeating("ApplyBurnDamage", 0, 0.25f);
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        CancelInvoke("ApplyBurnDamage");
    }
}   
