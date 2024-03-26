using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *  Скрипт генерирующий случайную награду после уровня.
 */
public class CardGenerator : MonoBehaviour
{
    [SerializeField] CardScriptableObject[] _cardStats;
    [SerializeField] private GameObject[] _cards;


    public void Awake()
    {
        ShuffleCards(_cardStats);
        GenerateCards();
    }



    public void GenerateCards()
    {
        // Устанавливаем случайные карты для выбора
        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].GetComponent<CardController>().SetCard = _cardStats[i];
        }
    }

    // Реализуем алгоритм тасования Кнута для генерации случайной награды
    private CardScriptableObject[] ShuffleCards(CardScriptableObject[] _cardStats)
    {
        for(int i = 0; i < _cardStats.Length; i++)
        {
            int r = Random.Range(i, _cardStats.Length);
            (_cardStats[i], _cardStats[r]) = (_cardStats[r], _cardStats[i]);
        }

        return _cardStats;
    }





    


}
