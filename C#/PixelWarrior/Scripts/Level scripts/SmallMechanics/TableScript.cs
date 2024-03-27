using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 /*
 * ��������� ��������������� ������ � ���������
 */
public class TableScript : MonoBehaviour
{
    
    [SerializeField] private GameObject table;
    [SerializeField] private GameObject UI;
    private GameObject _tableHandle;


    private void OnCollisionEnter2D(Collision2D collision) // ����� ����� �������� � ��������
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (_tableHandle == null)
            {
                _tableHandle = Instantiate(table,UI.transform);
                _tableHandle.transform.SetParent( UI.transform);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision) // ����� ����� ������ �� ��
    {
        if (_tableHandle != null)
            Destroy(_tableHandle);
    }


}
