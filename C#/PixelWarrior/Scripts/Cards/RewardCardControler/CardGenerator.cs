using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *  ������ ������������ ��������� ������� ����� ������.
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
        // ������������� ��������� ����� ��� ������
        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].GetComponent<CardController>().SetCard = _cardStats[i];
        }
    }

    // ��������� �������� ��������� ����� ��� ��������� ��������� �������
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
