using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 /*
 * Управляет взаимодействием игрока с табличкой
 */
public class TableScript : MonoBehaviour
{
    
    [SerializeField] private GameObject table;
    [SerializeField] private GameObject UI;
    private GameObject _tableHandle;


    private void OnCollisionEnter2D(Collision2D collision) // Когда игрок подходит к табличке
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
    private void OnCollisionExit2D(Collision2D collision) // Когда игрок уходит от неё
    {
        if (_tableHandle != null)
            Destroy(_tableHandle);
    }


}
