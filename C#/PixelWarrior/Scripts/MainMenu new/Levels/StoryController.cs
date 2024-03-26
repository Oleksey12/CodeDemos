using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/*
 * Скрипт управляет запуском историй в главном меню
 */

public class StoryController : MonoBehaviour
{

    [SerializeField] private GameObject _footer;
    [SerializeField] private GameObject _gameManager;
    [SerializeField] private TMP_Text _difficultyText;
    [SerializeField] private string _storySceneName;
    [SerializeField] private Sprite[] _difficultyImages;

    DifficultyList _head;
    private Image _footerSprite;
    float _gameDifficulty;

    private BaseValues _levelData;
    private PlayerData _playerData;

    
    void Start()
    {
        HashAllComponents();

        CreateDifficultyList(ref _head);

        GetListData(_head);
    }



    private void CreateDifficultyList(ref DifficultyList head)
    {
        head = new DifficultyList(1.0f, "EASY", _difficultyImages[0]);
        head.AddNewElement(new DifficultyList(1.1f, "NORMAL", _difficultyImages[1]))
            .AddNewElement(new DifficultyList(1.2f, "HARD", _difficultyImages[2]))
            .AddNewElement(new DifficultyList(1.4f, "INSANE", _difficultyImages[3]));
    }
    public void OnRightButtonClick() {_head = _head._next; GetListData(_head); }

    public void OnLeftButtonClick() { _head = _head._previous; GetListData(_head); }



    public void OnStartButtonClick()
    {

        _levelData.Difficulty = _gameDifficulty;
        _levelData.SaveStartParams();
        _playerData.SaveStartParams();

        SceneManager.LoadScene(_storySceneName);
    }
    private void GetListData(DifficultyList node)
    {
        _difficultyText.text = node._difficultyName;
        _gameDifficulty = node._difficultyVal;
        _footerSprite.sprite = node._difficultyImage;
    }

    private void HashAllComponents()
    {
        _footerSprite = _footer.GetComponent<Image>();
        _levelData = _gameManager.GetComponent<BaseValues>();
        _playerData = _gameManager.GetComponent<PlayerData>();
    }
}
