using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VKBillBoard : MonoBehaviour
{
    void Start()
    {
        
        // ������� ������������� �������� �� ���������, ��� ����������� �������
        MainManager.vk.GetImage((Texture2D texture) =>
        {
            gameObject.GetComponent<Image>().sprite = Sprite.Create(texture,
                new Rect(0,0,texture.width,texture.height),
                new Vector2(0.5f, 0.5f));
        });
       
    }


}
