using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ������ ��� �������� �� ������� ��������� ���� � ����� ����
 */

public class SelfDestruction : MonoBehaviour
{
    [SerializeField] private Canvas animationCanvas;
    private Animator _animator;

    private IBuildProduct _cardSelectUI;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _cardSelectUI = GetComponent<IBuildProduct>();
    }

    // ��� ���� ���������� �������� ��������� ����
    void FixedUpdate()
    {
        // ����� ������������ �������� ��������� ����� � ��������� ��������� ���� � ������ ����� � ������� ����
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("EndState"))
        {
            
            _cardSelectUI.CreateProduct();
            animationCanvas.enabled = false;
            Destroy(gameObject);
            
        }

    }
}
