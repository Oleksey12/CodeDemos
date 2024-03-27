using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 *  ������, ����������� ���������� �������� ����
 */
public class MainMenuManager : MonoBehaviour
{
    // ���������� ��������
    private ICloudManager cloudManager;
    // ���������� ������������ �����
    private IColorChangeEffect colorChange;


    private void Start()
    {
        // �������� ������ �� ������ ��������
        HashComponents();

    }



    void FixedUpdate() // ��������� ��������� �������� ����
    {
        // �������� �������, ����������� ������������ ������
        colorChange.ColorChanger();

        // ������������ �������� �����
        cloudManager.CloudController();
    }


    private void HashComponents()
    {
        cloudManager = GetComponent<ICloudManager>();
        colorChange = GetComponent<IColorChangeEffect>();
    }
}
