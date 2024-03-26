using UnityEngine;

/*
 * Скрипт реализует реакцию босса на получение урона
 */

public class BossReact : HitReact
{
    [SerializeField] protected float _speedMultiplier = 1.4f;
    [SerializeField] protected float _sizeDecrease = 0.5f;
    protected float _normalSpeed;
    protected float _maxSize;

    protected override void Start()
    {
        base.Start();
        _normalSpeed = _characteristics.Speed;
        _maxSize = _characteristics.Size;

    }
    protected override void HashComponents()
    {
        base.HashComponents();
        _characteristics = GetComponent<IBotCharacteristics>();
    }

    public override void ReactToHit(Vector3 senderPosition)
    {
        base.ReactToHit(senderPosition);

        // Скорость босса по мере потери им хп возрастает от 1(100% хп) до _speedMultiplier (0% хп)
        _characteristics.Speed = _normalSpeed * (1 + (1 - _healthScript.Current_health / _healthScript.Max_health) * (_speedMultiplier - 1));
        // Размер босса изменяется от _maxSize(100% хп) до _sizeDecrease *  _maxSize(0% хп)
        _characteristics.Size = _maxSize * (1 + (1 - _healthScript.Current_health / _healthScript.Max_health) * (_sizeDecrease - 1));
    }


    protected override void DamageAnimation()
    {
        if(!_animator.GetBool("IsCasting"))
            base.DamageAnimation();
    }
}

