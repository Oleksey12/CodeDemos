
using UnityEngine;
/*
 * Скрипт реализует действия бота при получении урона
 */
public class HitReact : MonoBehaviour, IReactToHit
{
    [SerializeField] protected ParticleSystem _particleSystem;
    protected IHealthScript _healthScript;
    protected IPlayerDefaultData _playerData;
    protected Rigidbody2D _botRb;
    protected Animator _animator;
    protected IBotCharacteristics _characteristics;
    protected IKnockbackController _botKnockbackController;

    protected virtual void Start()
    {
        HashComponents();
    }


    public virtual void ReactToHit(Vector3 senderPosition)
    {
        ApplyKnockback(gameObject.transform.position, senderPosition, _botRb);

        _particleSystem.Play();
        DamageAnimation();

        _healthScript.ChangeCurrentHealth(-_playerData.Damage);

    }

    protected virtual void DamageAnimation()
    {
        // Проигрываем анимацию получения урона ровно 1 раз и возвращаемся к обычному состоянию

        _animator.SetBool("DamageAnimation", true);
        Invoke("StopAnimation", 0.4f);
    }

    protected virtual void StopAnimation()
    {
        if(_animator.GetBool("DamageAnimation"))
            _animator.SetBool("DamageAnimation", false);
    }


    protected void ApplyKnockback(Vector3 botPos,Vector3 collisionPos, Rigidbody2D botRb)
    {
        // Игрок нанёсший удар находился справа
        if (collisionPos.x > botPos.x)
            _botKnockbackController.ApplyKnockback(new Vector2(-_playerData.PlayerKnockback * _characteristics.InputKnockBack, 0));
        else // Игрок нанёсший удар находился слева
            _botKnockbackController.ApplyKnockback(new Vector2(_playerData.PlayerKnockback * _characteristics.InputKnockBack, 0));

    }

    protected virtual void HashComponents()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _animator = GetComponent<Animator>();
        _botRb = GetComponent<Rigidbody2D>();
        _botKnockbackController = GetComponent<IKnockbackController>();
        _healthScript = GetComponent<IHealthScript>();
        _playerData = FindObjectOfType<PlayerData>();
    }
}

