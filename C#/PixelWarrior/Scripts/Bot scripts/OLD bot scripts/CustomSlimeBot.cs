using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
/*
 * Скрипт 3-го уровня в наследовании, изменяет параметры бота на рандомные
 * в зависимости от сгенерированной для него породы.
 */

public class CustomSlimeBot : SlimeBot
{
    protected string _filePath = "GameParams.txt";
    [SerializeField] protected Animator _botAnimator;
    protected float _difficultyLevel = 1.0f;
    protected Sprite _botType1;
    protected Sprite _botType2;
    protected Sprite _botType3;
    protected float _botSize;

    protected virtual int RandomType() // Рандомно определяет какого вида будет бот
    {
        int roll = UnityEngine.Random.Range(0, 10);

        // 1- 60% 2 -30% 3- 10%
        if (roll < 6)
            return 1;
        else if (roll < 9)
            return 2;
        else
            return 3;

    }
    protected virtual void SetSprite(GameObject botObj, Sprite newSprite) // Меняет спрайт бота на тот, что соответствует его породе
    {
        botObj.GetComponent<SpriteRenderer>().sprite = newSprite;
    }
    protected virtual void IncreaseScale(GameObject botObj, float increase) // Изменяет размер бота в зависимости от породы
    {
        botObj.transform.localScale = new Vector3(_botSize * increase, botSettings.Size * increase, botSettings.Size);
    }
    protected virtual void IncreaseStats(GameObject botObj, ParticleSystem hitParticles, float increase) // Изменяет параметры бота в зависимости от породы
    {
        // Увеличиваем базовые параметры в зависимости от уровня сложности и породы бота
        _botDamage *= increase * _difficultyLevel;
        _botSpeed *=  _difficultyLevel / increase;


        // Увеличиваем хп бота в зависимости от уровня сложности и породы бота
        botObj.GetComponent<HealthScript>().setHealth
            (botObj.GetComponent<HealthScript>().Max_health * increase * _difficultyLevel,
            botObj.GetComponent<HealthScript>().Max_health * increase * _difficultyLevel);

        // Увеличиваем размер
        IncreaseScale(botObj, increase);


    } 
    protected virtual void SetBotBreed(GameObject botObj) // Определяет породу бота
    {
        int botType = RandomType();

        _botAnimator.SetInteger("BotType", botType);
        switch (botType)
        {
            case 1:
                {
                    SetSprite(botObj, _botType1);
                    IncreaseStats(botObj,_hitParitcles, UnityEngine.Random.Range(0.95f, 1.2f));
                    break;
                }
            case 2:
                {
                    SetSprite(botObj, _botType2);
                    IncreaseStats(botObj,_hitParitcles, UnityEngine.Random.Range(1.3f, 1.7f));
                    break;
                }
            case 3:
                {
                    SetSprite(botObj, _botType3);
                    IncreaseStats(botObj,_hitParitcles, UnityEngine.Random.Range(1.8f, 2.5f));
                    break;
                }
        }

    }
    protected override void Start()
    {
        base.Start();
        // Генерируем породу для бота
        SetBotBreed(gameObject);
    }
    protected override void GetValues(BotScriptableObject settings)
    {
        base.GetValues(settings);
        // Cчитываем ещё и косметические параметры бота: спрайт, размер
        _botType1 = settings.BotType1;
        _botType2 = settings.BotType2;
        _botType3 = settings.BotType3;
        _botSize = botSettings.Size;
        
        _difficultyLevel = FindObjectOfType<BaseValues>().Difficulty;
    }

    protected override Vector3 DefaultMovement(Vector3 curentBotPosition, float moveSpeed) // Движение без игрока
    {
        // В отличии от предыдущей функции движения бота, эта ещё и управляет его спрайтом (смотрит в бот вправо или влево)
        switch (_botDirection)
        {
            case 0: // Бот двигается вправо
                {
                    _botAnimator.SetBool("GoingLeft", false);
                    return new Vector3(curentBotPosition.x +
                            moveSpeed / 1.5f, curentBotPosition.y, 0);

                }
            case 1: // Бот движется влево
                {
                    _botAnimator.SetBool("GoingLeft", true);
                    return new Vector3(curentBotPosition.x -
                            moveSpeed / 1.5f, curentBotPosition.y, 0);

                }
        }
        return curentBotPosition;

    }
    protected override Vector2 BotMovement(Vector3 playerCoords, float moveSpeed) // Приследование игрока
    {

        Vector2 result =  base.BotMovement(playerCoords, moveSpeed);

        // В отличии от предыдущей функции движения бота, эта ещё и управляет его спрайтом (смотрит в бот вправо или влево)
        if(_moveDirection.x > 0)
            _botAnimator.SetBool("GoingLeft", false);
        else
            _botAnimator.SetBool("GoingLeft", true);
        return result;
    }


}
