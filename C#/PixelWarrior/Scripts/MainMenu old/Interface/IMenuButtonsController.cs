using UnityEngine;
/*
 * Управляет функционалом кнопок главного меню
 */ 
public interface IMenuButtonsController
{
    // Кнопка начала игры
    public void OnPlayClick();

    // Кнопка выхода
    public void OnExitButtonClick();

}