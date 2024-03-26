using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OnSlimeCollision : MonoBehaviour, IOnOtherBotCollision
{
    IBotCharacteristics _characteristics;
    protected IKnockbackController _botKnockbackController;
    Rigidbody2D _body2D;

    private void Start()
    {
        _body2D = GetComponent<Rigidbody2D>();
        _characteristics = GetComponent<IBotCharacteristics>();
    }


    public void RegisterCollision(GameObject collisionObject)
    {
   
        // Если другой бот находится справа
        if (collisionObject.transform.position.x > gameObject.transform.position.x)
        {
            // Откидываем ботов в разные стороны
            _body2D.velocity += new Vector2(-_characteristics.CollisionKnockback, 0);
        }
        else
        {
            _body2D.velocity += new Vector2(_characteristics.CollisionKnockback, 0);
        }
    }
}

