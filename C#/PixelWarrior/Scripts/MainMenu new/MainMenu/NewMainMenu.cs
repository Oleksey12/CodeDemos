using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Скрипт управляет переходами из главного меню в другие окна
 */
public class NewMainMenu : MonoBehaviour
{
    [SerializeField] GameObject _SettingsObject;
    [SerializeField] GameObject _LevelMenuObject;
    [SerializeField] GameObject _SkinsMenuObject;

    public void ExitButton() => Application.Quit();


    public void SkinsButton()
    {
        _SkinsMenuObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SettingsButton()
    {
        _SettingsObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void PlayButton()
    {
        _LevelMenuObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
