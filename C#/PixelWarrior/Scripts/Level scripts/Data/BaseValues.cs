using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
 *  Cкрипт управляет данными текущей игры
 */
public class BaseValues : MonoBehaviour, IGameDataScript
{
    public const string difficulty = "Difficulty";
    protected float _difficultyVal = 1.0f;

    public const string showFPS = "showFPS";
    protected float _showFPS = 1f;

    public const string showVFX = "showVFX";
    protected float _showVFX = 1f;

    // Переменная, хранящая все данные игры в удобном для работы с файлами виде
    /*
     * Взаимодействие с файлом ведём через эту переменную, так как
     * гораздо проще один раз открыть файл и ввести(или извлечь) сразу все данные,
     * чем много раз открывать и закрывать файл вводя или считывая по одной переменной
     */
    private Dictionary<string, float> gameParametrs;

    // Имя файла, хранящего данные
    private string _filePath = "GameParams.txt";
    private IFileSystem fileSystem;
    

    public float Difficulty
    {
        get => _difficultyVal;
        set => _difficultyVal= value;
    }

    public string FilePath
    {
        get => _filePath;
        set => _filePath = value;
    }

    public float ShowFPS
    {
        get=> _showFPS;
        set => _showFPS = value;
    }
    public float ShowVFX
    {
        get => _showVFX;
        set => _showVFX = value;
    }


    private void Awake()
    {
        fileSystem = gameObject.GetComponent<IFileSystem>();
    }
    public void SaveStartParams()
    {

        gameParametrs = new Dictionary<string, float>();
        try
        {
            gameParametrs.Add(difficulty, _difficultyVal);
            gameParametrs.Add(showFPS, _showFPS);
            gameParametrs.Add(showVFX, _showVFX);

            fileSystem.SaveData(gameParametrs, _filePath);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void GetAllParams()
    {
        try
        {
            gameParametrs = fileSystem.GetParamsFromFile(_filePath);
            _difficultyVal = gameParametrs[difficulty];
            _showFPS = gameParametrs[showFPS];
            _showVFX = gameParametrs[showVFX];
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        
    }


}
