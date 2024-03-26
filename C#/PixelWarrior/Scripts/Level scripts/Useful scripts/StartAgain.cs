using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Cкрипт управляет меню после прохождения истории
 */
public class StartAgain : MonoBehaviour
{
    public void LoadStartScene() => SceneManager.LoadScene("NewMainMenu");
}
