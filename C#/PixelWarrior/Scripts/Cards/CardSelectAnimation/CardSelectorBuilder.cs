using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/*
 *  Скрипт создаёт холст с выбором карт
 */
public class CardSelectorBuilder : MonoBehaviour, IBuildProduct
{

    [SerializeField] private Canvas cardCanvas;


    public void CreateProduct() 
    {
        Instantiate(cardCanvas);
    }
}

