using System;
using UnityEngine;

/*
 * √лавный скрипт, управл€ющий всеми компонентами бота
 * –еализует модель поведени€ слайма.
 */
public class SlimeAI : IBotAI, IReactiveTarget
{
    
    protected IApplyBotBreed _applyBotBreed;
    protected IBotBehaviour _botBehaviour;
    protected SpriteRenderer _rend;
    protected IKnockbackController _slimeKnockbackController;
    protected IReactToHit _reactToHit;
    protected IOnPlayerCollision _playerCollision;
    protected IOnBlockCollision _blockCollision;
    protected IOnOtherBotCollision _botCollision;

    
    protected override void Start()
    {
        base.Start();

        _applyBotBreed.SetBreed();

        SetStats();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        switch (collision.tag)
        {
            case ("Player"):
                {
                    _playerCollision.RegisterCollision(collision.gameObject);
                    break;
                }
            case ("block"):
                {
                    _blockCollision.RegisterCollision();
                    break;
                }
            case ("Slime"):
            {
                _botCollision.RegisterCollision(collision.gameObject);
                break;
            }
        }
    }

    protected override void FixedUpdate()
    {
        if(!_slimeKnockbackController.isDamaged)
            _botBehaviour.Behave();
        else
            _slimeKnockbackController.SlowDownEffect();
    }

    protected override void SetStats()
    {
        _botHealth.setHealth(_characteristics.Hp, _characteristics.Hp);
        gameObject.transform.localScale = new Vector3(_characteristics.Size, _characteristics.Size, _characteristics.Size);

        if (_characteristics.BotSprite != null)
            _rend.sprite = _characteristics.BotSprite;
        else
            _rend.sprite = _characteristics.BotSprites[0];
    }


    protected override void HashAllSubscripts()
    {
        base.HashAllSubscripts();
        _botBehaviour = GetComponent<IBotBehaviour>();
        _applyBotBreed = GetComponent<IApplyBotBreed>();
        _rend = GetComponent<SpriteRenderer>();
        _reactToHit = GetComponent<IReactToHit>();
        _slimeKnockbackController = GetComponent<IKnockbackController>();
        _playerCollision = GetComponent<IOnPlayerCollision>();
        _blockCollision = GetComponent<IOnBlockCollision>();
        _botCollision = GetComponent<IOnOtherBotCollision>();

    }

    public void React(Vector3 senderPosition)
    {
        _reactToHit.ReactToHit(senderPosition);
    }
}

