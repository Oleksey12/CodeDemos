using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Скрипт отображает частоту кадров в интерфейсе пользователя
 */
public class FPS_Display : MonoBehaviour
{
    // Текстовое поле, в котором будут храниться данные о текущей частоте кадров
    [SerializeField] private Text fpsLabel;

    private BaseValues _levelValues;


    private float _currentDelay;
    private float _updateTime = 0.5f;

    void Start()
    {
        _levelValues = FindObjectOfType<BaseValues>();


        if(_levelValues.ShowFPS == 1.0f)
            InvokeRepeating("DisplayFrameCount", 0.5f, _updateTime);
    }
    

    private void DisplayFrameCount()
    {
        fpsLabel.text = Convert.ToString((int)(1 / _currentDelay));
    }

    void Update()
    {
        _currentDelay = Time.deltaTime;
    }
}
