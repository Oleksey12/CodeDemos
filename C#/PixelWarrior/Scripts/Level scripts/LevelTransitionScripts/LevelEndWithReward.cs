using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
 *  Класс расширяет функцию конца уровня предудщего скрипта, добавляя ещё выдачу карт игроку
 */
class LevelEndWithReward : LevelEndManager
{
    ICardAppearAnimation appearAnimation;

    protected override void Start()
    {
        base.Start();
        // Получаем ссылку на компонент
        appearAnimation = GetComponent<ICardAppearAnimation>();
    }
    protected override void LevelEndFunction()
    {
        base.LevelEndFunction();
        // Вдобавок запускаем анимацию появления карт
        appearAnimation.StartAnimation();
    }
}

