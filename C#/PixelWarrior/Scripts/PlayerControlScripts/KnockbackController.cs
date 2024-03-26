using UnityEngine;
/*
 *  Скрипт управляет эффектом резкой отдачи
 */
public class KnockbackController : MonoBehaviour, IKnockbackController
{
    public bool isDamaged { get; set; } = false;
    private Rigidbody2D _playerRb;
    private IVelocityController _velocityController;
    private IPlayerDefaultData _playerData;
    private Animator _animator;
    private void Start()
    {
        HashComponents();
    }



    public void ApplyKnockback(Vector2 knockback)
    {
        _playerRb.velocity = knockback;
        isDamaged = true;
        _animator.Play("Damage");
        
    }
    public void SlowDownEffect()
    {
        // Объект находится в состоянии получения урона, пока эффект откидывания не закончится
        if (_playerRb.velocity != Vector2.zero)
            _velocityController.VecloityChange(_playerData.VelocityReduceAmount);
        else
            isDamaged = false;

    }

    private void HashComponents()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _velocityController = GetComponent<VelocityController>();
        _playerData = GetComponent<IPlayerDefaultData>();
        _animator = GetComponent<Animator>();
    }
}


