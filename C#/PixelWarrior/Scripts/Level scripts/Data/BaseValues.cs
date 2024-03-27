using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
 *  C����� ��������� ������� ������� ����
 */
public class BaseValues : MonoBehaviour, IGameDataScript
{
    public const string difficulty = "Difficulty";
    protected float _difficultyVal = 1.0f;

    public const string showFPS = "showFPS";
    protected float _showFPS = 1f;

    public const string showVFX = "showVFX";
    protected float _showVFX = 1f;

    // ����������, �������� ��� ������ ���� � ������� ��� ������ � ������� ����
    /*
     * �������������� � ������ ���� ����� ��� ����������, ��� ���
     * ������� ����� ���� ��� ������� ���� � ������(��� �������) ����� ��� ������,
     * ��� ����� ��� ��������� � ��������� ���� ����� ��� �������� �� ����� ����������
     */
    private Dictionary<string, float> gameParametrs;

    // ��� �����, ��������� ������
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
