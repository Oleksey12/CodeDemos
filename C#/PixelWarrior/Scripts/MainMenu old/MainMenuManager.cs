using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 *  Скрипт, управляющий анимациями главного меню
 */
public class MainMenuManager : MonoBehaviour
{
    // Управление облаками
    private ICloudManager cloudManager;
    // Управление переливанием цвета
    private IColorChangeEffect colorChange;


    private void Start()
    {
        // Получаем ссылку на классы скриптов
        HashComponents();

    }



    void FixedUpdate() // Управляем эффектами главного меню
    {
        // Вызываем функцию, управляющую переливанием текста
        colorChange.ColorChanger();

        // Осуществляем движение облак
        cloudManager.CloudController();
    }


    private void HashComponents()
    {
        cloudManager = GetComponent<ICloudManager>();
        colorChange = GetComponent<IColorChangeEffect>();
    }
}
