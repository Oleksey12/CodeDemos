using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Скрипт для перехода из события появления карт в выбор карт
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

    // Ждём пока закончится анимация появления карт
    void FixedUpdate()
    {
        // После проигрывания анимации отключает холст с анимацией появления карт и создаёт холст с выбором карт
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("EndState"))
        {
            
            _cardSelectUI.CreateProduct();
            animationCanvas.enabled = false;
            Destroy(gameObject);
            
        }

    }
}
