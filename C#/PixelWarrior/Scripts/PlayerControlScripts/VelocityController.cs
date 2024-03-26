using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

internal class VelocityController : MonoBehaviour, IVelocityController
{


    private Rigidbody2D _objectRb;
    private void Start()
    {
        _objectRb = GetComponent<Rigidbody2D>();
    }

    private Vector2 DecreasedVelocity(Vector2 velocity, float amount) // Уменьшает скорость игрока 
    {
        Vector2 velocityDecrease = velocity.normalized * amount;
        return velocity - velocityDecrease;
    }
    public void VecloityChange(float amount) // Для симуляции резкого откидывания 
    {
        if (Mathf.Abs(_objectRb.velocity.x) <= amount && Mathf.Abs(_objectRb.velocity.y) <= amount)
            _objectRb.velocity = Vector2.zero;
        else
            _objectRb.velocity =  DecreasedVelocity(_objectRb.velocity, amount);
    }


}