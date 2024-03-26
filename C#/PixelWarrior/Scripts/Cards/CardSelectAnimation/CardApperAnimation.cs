using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
 * Скрипт создаёт холст, проигрывающий анимацию появления карт
 */
public class CardApperAnimation : MonoBehaviour, ICardAppearAnimation
{
    [SerializeField] Canvas animationCanvas;
    public void StartAnimation()
    {
        Instantiate(animationCanvas);
    }
}

