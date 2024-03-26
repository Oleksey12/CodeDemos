using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Движение слайма в отсутсвии игрока
 */
public class SlimeDefaultMovement : MonoBehaviour, IDefaultMovement
{
    protected IBotCharacteristics _characteristics;
    protected Rigidbody2D _rb;
    private void Start()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _rb = GetComponent<Rigidbody2D>();
    }


    public Vector2 Move() 
    {
        Vector2 moveVector = new Vector2(_characteristics.Direction.x, 0).normalized
            * _characteristics.Speed
            * Time.deltaTime;

        _rb.velocity += moveVector;
        return moveVector;
    }
}

