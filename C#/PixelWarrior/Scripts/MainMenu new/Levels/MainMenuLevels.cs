using UnityEngine;


public class MainMenuLevels : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;


    public void BackButton()
    {
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}

