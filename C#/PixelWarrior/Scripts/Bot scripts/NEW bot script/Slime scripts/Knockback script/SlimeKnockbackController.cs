using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Скрипт управляет эффектом отдачи при нанесении урон боту
 */
public class SlimeKnockbackController : MonoBehaviour, IKnockbackController
{
    public bool isDamaged { get; set; } = false;
    private Rigidbody2D _botRb;
    private IVelocityController _velocityController;
    private IBotCharacteristics _charactersitics;
    private void Start()
    {
        _botRb = GetComponent<Rigidbody2D>();
        _velocityController = GetComponent<VelocityController>();
        _charactersitics = GetComponent<IBotCharacteristics>();
    }
    public void ApplyKnockback(Vector2 knockback)
    { 
        isDamaged = true;
        _botRb.velocity += knockback;
    }
    public void SlowDownEffect()
    {
        // Бот находится в состоянии получения урона, пока эффект откидывания не закончится
        if (_botRb.velocity != Vector2.zero)
            _velocityController.VecloityChange(_charactersitics.VelocityDecrease);
        else
            isDamaged = false;

    }

}
