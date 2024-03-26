using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*
 * Скрипт реализует изменение параметров бота, взависимости от его породы
 */
public class ApplySlimeBreed : MonoBehaviour, IApplyBotBreed
{
    private IBotCharacteristics _characteristics;
    private BaseValues _levelValues;
    [SerializeField] Animator animator;
    [SerializeField] private float minStartStats = 0.9f;
    [SerializeField] private float maxStartStats = 1.1f;

    private void Awake()
    {
        HashAllComponents();
    }

    public void SetBreed() // Реализация функции установки породы бота
    {
        // Переменная хранит номер породы бота
        int botType;
        // Коэффиценты увеличения(уменьшения) характеристик бота
        float difficultyStatsMultiplicator;
        float botTypeStatsMultiplicator;

        botType = GenerateBotType(_characteristics.BotSprites.Length);
        difficultyStatsMultiplicator = _levelValues.Difficulty;
        botTypeStatsMultiplicator = CountBotTypeMultiplicator(botType, minStartStats, maxStartStats);

        ChangeBotStatsByItsBreed(difficultyStatsMultiplicator, botTypeStatsMultiplicator);
        ChangeBotAppearence(botType);


    }

    private void ChangeBotAppearence(int botType)
    {
        // Если у бота есть другие породы
        if(_characteristics.BotSprites.Length != 1)
            animator.SetInteger("BotType", botType);
        _characteristics.BotSprite = _characteristics.BotSprites[botType - 1];
    }

    private void HashAllComponents()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _levelValues = FindObjectOfType<BaseValues>();
    }

    private int GenerateBotType(int typeCount)
    {
        // Разделяем шансы так: шанс на 1 тип n*minChane, на второй n-1*minChane , на третий n-2*minChane и т.д
        float minChance = 1f / ((1 + typeCount) * typeCount / 2f);
        float randNum = UnityEngine.Random.Range(0, 1f);
        for (int i = 0; i < typeCount; ++i)
        {
            randNum -= minChance * (typeCount - i);
            if (randNum <= 0)
                return i + 1;
        }
        return typeCount;
    }
    private float CountBotTypeMultiplicator(int botType,float minStartStats, float maxStartStats)
    {
        return UnityEngine.Random.Range(minStartStats,maxStartStats) + ((float)botType-1)/2;
    }

    

    private void ChangeBotStatsByItsBreed(float difficultyMultiplicator , float breedMultiplicator)
    {
        // Увеличиваем базовые параметры в зависимости от уровня сложности и породы бота
        _characteristics.Damage *= difficultyMultiplicator * breedMultiplicator;
        _characteristics.Speed *= difficultyMultiplicator / breedMultiplicator;


        _characteristics.InputKnockBack /= breedMultiplicator;
        _characteristics.CollisionKnockback /= breedMultiplicator;

        _characteristics.Hp *= difficultyMultiplicator * breedMultiplicator;
        _characteristics.Size *= breedMultiplicator;
        gameObject.transform.localScale = new Vector3(_characteristics.Size, _characteristics.Size, _characteristics.Size);
    }

}

