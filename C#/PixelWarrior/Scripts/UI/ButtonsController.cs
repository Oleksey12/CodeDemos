using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonsController : MonoBehaviour, IButtonsController
{
    private PlayerController playerController;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // ‘ункции выполн€ющиес€ при нажатии кнопок:
    public void OnDashButton() => playerController.OnDashButtonClick();
    public void OnAttackButton() => playerController.OnAttackButtonClick();
    public void OnMenuButton() => SceneManager.LoadScene("NewMainMenu");
}
