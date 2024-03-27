using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * ������ ���������� ������� ������ � ���������� ������������
 */
public class FPS_Display : MonoBehaviour
{
    // ��������� ����, � ������� ����� ��������� ������ � ������� ������� ������
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
