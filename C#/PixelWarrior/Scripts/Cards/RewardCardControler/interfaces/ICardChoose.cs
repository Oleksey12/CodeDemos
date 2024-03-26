using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 *  Интерфейс определяет действия после выбора карты
 */
public interface ICardChoose 
{

    // Когда эта карта выбрана
    public void OnCardChoose(CardScriptableObject stats);

}

