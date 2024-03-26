using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
 * Скрипт управляет параметрами игрока
*/
public class PlayerData : IPlayerDefaultData, IGameDataScript
{
    // Переменная, хранящая все данные игрока в удобном для работы с файлами виде
    private Dictionary<string, float> _playerParametrs;

    // Имя файла с хранимыми данными
    private string _filePath = "PlayerParams.txt";
    private IFileSystem fileSystem;

    private void Awake()
    {
        fileSystem = gameObject.GetComponent<IFileSystem>();
    }
    public string FilePath
    {
        get => _filePath;
        set => _filePath = value;
    }


    public void GetAllParams() // Извлекаем все значения характеристик из файла игрока
    {
        try
        {
            _playerParametrs = fileSystem.GetParamsFromFile(_filePath);
            _maxHpVal = _playerParametrs[maxHp];
            _curHpVal = _playerParametrs[curHp];
            _damageVal = _playerParametrs[damage];
            _speedVal = _playerParametrs[speed];
            _attackCooldownVal = _playerParametrs[attackCooldown];
            _dashCooldownVal = _playerParametrs[dashCooldown];
            _playerKnockbackVal = _playerParametrs[playerKnockback];
            _velocityReduceVal = _playerParametrs[velocityReduce];
            _dashPowerVal = _playerParametrs[dashPower];
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public void SaveStartParams()
    {
        _playerParametrs = new Dictionary<string, float>();


        _playerParametrs.Add(maxHp, _maxHpVal);
        _playerParametrs.Add(curHp, _curHpVal);
        _playerParametrs.Add(damage, _damageVal);
        _playerParametrs.Add(speed, _speedVal);
        _playerParametrs.Add(attackCooldown, _attackCooldownVal);
        _playerParametrs.Add(dashCooldown, _dashCooldownVal);
        _playerParametrs.Add(playerKnockback, _playerKnockbackVal);
        _playerParametrs.Add(velocityReduce, _velocityReduceVal);
        _playerParametrs.Add(dashPower, _dashPowerVal);

        fileSystem.SaveData(_playerParametrs, _filePath);
    }
}
