using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * ������ ��� �������������� ��������� �������� ������ � �����
 */
public class SelectUICamera : MonoBehaviour
{
    private void Start()
    {

        Camera mainCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        var canv = gameObject.GetComponent<Canvas>();
        canv.worldCamera = mainCamera;
    }

}
