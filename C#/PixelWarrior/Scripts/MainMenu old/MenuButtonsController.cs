using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * Скрипт управляет функционалом кнопкок главного меню
 */
public class MenuButtonsController : MonoBehaviour, IMenuButtonsController
{
    [SerializeField] private GameObject difficultyPicker;
    [SerializeField] private Canvas parentCanv;
    [SerializeField] private Canvas uicanv;

    private GameObject _tempPicker;
    private ICloudManager cloudManager;

    private void Awake()
    {
        // Получаем ссылку на классы используемых скриптов
        cloudManager = FindObjectOfType<CloudManager>();
    }


    // Кнопка начала игры
    public void OnPlayClick() => OpenDifficultyPicker();

    // Кнопка выхода
    public void OnExitButtonClick() => Application.Quit();

    public void OnBossLoadClick() => SceneManager.LoadScene("BossFight"); // Секретная кнопка не трогать





    private void OpenDifficultyPicker()
    {
        if (_tempPicker == null)
        {
            InstantiateDifficultyPicker();
            ToggleCanvasOff(uicanv);
        }
    }

    private void InstantiateDifficultyPicker()
    {
        _tempPicker = Instantiate(difficultyPicker);
        _tempPicker.transform.SetParent(parentCanv.transform);
        _tempPicker.transform.position = new Vector3(_tempPicker.transform.position.x + Screen.width / 2, _tempPicker.transform.position.y + Screen.height / 2, 0);
        _tempPicker.transform.localScale = new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1);
    }

    private void ToggleCanvasOff(Canvas canv)
    {
        canv.enabled = false;
    }



}