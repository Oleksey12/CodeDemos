using UnityEngine;
using UnityEngine.UI;
/*
*  Скрипт управляет картой таинственной награды
*/
class CardController : MonoBehaviour
{
    ICardChoose _cardPicker;

    // Поле хранит параметры таинственной награды текущей карты
    private CardScriptableObject _card;

    public CardScriptableObject SetCard
    {
        get => _card;
        set { _card = value; GetComponent<Image>().sprite = _card.Card; }
    }


    private void Start()
    {
        _cardPicker = GetComponent<ICardChoose>();
    }


    // При выборе пользователем этой карты
    public void OnCardClick()
    {
        _cardPicker.OnCardChoose(_card);
    }
}

