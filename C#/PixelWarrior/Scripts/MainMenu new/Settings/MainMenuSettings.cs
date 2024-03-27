using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
/*
* ������ ��������� ������� ���� "���������".
*/

public class MainMenuSettings : MonoBehaviour
{
    private bool _showFps = true;
    private bool _showVFX = true;


    private Resolution[] _resolutions;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    private BaseValues _lvlValues;

    private void Start()
    {
        // ������� ���� � ������� ����
        _lvlValues = FindObjectOfType<BaseValues>();
        GetScreenResolutions();
    }




    // ������ ���� ��������:
    public void FPSButton()
    {
        _showFps = !_showFps;
        _lvlValues.ShowFPS = _showFps ? 1 : 0;
    }

    public void VFXButton()
    {
        _showVFX = !_showVFX;
        _lvlValues.ShowVFX = _showVFX ? 1 : 0;
    }
    public void BackButton()
    {
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    // �������������� ������
    public void QualityButton(TMP_Dropdown quality) { QualitySettings.SetQualityLevel(quality.value); } 
    public void SetResolution(TMP_Dropdown resolution) { Screen.SetResolution(_resolutions[resolution.value].width, _resolutions[resolution.value].height, true); }

 

    // �������� ��� ��������� ���������� ������ ����������
    private void GetScreenResolutions()
    {
        int currentResolutionIndex = 0;
        // � ������ ������ ����� ��������� �������� ������ � ����� List<string>
        List<string> resolutionList = new List<string>();

        // �������� ���������� � ���� ��������� ����������� ������
        _resolutions = Screen.resolutions;
        Application.targetFrameRate = 60;

        for (int i = 0; i < _resolutions.Length; ++i)
        {
            if (_resolutions[i].width == Screen.width)
                currentResolutionIndex = i;

            resolutionList.Add(_resolutions[i].width + "x" + _resolutions[i].height);
        }


        // ���������� ��������� ���������� � ������ ������
        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(resolutionList);
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        

    }

}
