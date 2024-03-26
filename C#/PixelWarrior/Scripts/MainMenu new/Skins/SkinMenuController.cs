using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Кнопки окна "облики"
 */
public class SkinMenuController : MonoBehaviour
{
    [SerializeField] GameObject _mainMenu;

    public void OnBackButton()
    {
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
