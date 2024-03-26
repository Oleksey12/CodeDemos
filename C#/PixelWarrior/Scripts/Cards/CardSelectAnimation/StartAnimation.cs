using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/*
 * Создаёт холст, проигрывающий анимацию появления карт в конце уровня
 */ 
public class StartAnimation : MonoBehaviour, IBuildProduct
{
    [SerializeField] private Canvas animationCanvas;
    public void CreateProduct()
    {
        Instantiate(animationCanvas);
    }
}

