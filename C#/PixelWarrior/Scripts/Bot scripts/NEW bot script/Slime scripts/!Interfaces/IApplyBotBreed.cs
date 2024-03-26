using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Функционал интерфейса - изменение базовых характеристик бота, в зависимости от его породы
 */
public interface IApplyBotBreed
{
    // Функция изменяет характеристики бота, в зависимости от сгенерированной породы
    public void SetBreed();

}

