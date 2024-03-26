using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoveAnimation : MonoBehaviour, IMoveAnimation
{
    
    [SerializeField] Animator animator;
    Rigidbody2D _botRb;
    Transform _botTransform;
    private IBotCharacteristics _characteristics;
    protected IKnockbackController _slimeKnockbackController;
    private void Start()
    {
        HashComponents();
    }

    public void SetMoveDirection()
    {
        // Не меняем направление спрайта бота, если пока бот находится в состоянии получения урона
        if (!_slimeKnockbackController.isDamaged)
        {
            if (_botRb.velocity.x > 0.1f)
                _botTransform.localScale = new Vector3(-_characteristics.Size, _characteristics.Size, _characteristics.Size);
            else if( _botRb.velocity.x < -0.1f)
                _botTransform.localScale = new Vector3(_characteristics.Size, _characteristics.Size, _characteristics.Size);
        }
    }


    private void HashComponents()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _botRb = GetComponent<Rigidbody2D>();
        _botTransform = GetComponent<Transform>();
        _slimeKnockbackController = GetComponent<IKnockbackController>();
    }


}

