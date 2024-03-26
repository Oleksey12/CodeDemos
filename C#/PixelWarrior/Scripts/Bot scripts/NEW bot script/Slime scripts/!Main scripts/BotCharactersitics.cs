using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class BotCharactersitics : MonoBehaviour, IBotCharacteristics
{
    [SerializeField] protected SlimeStatsScriptableObject botSettings;

    protected string _tag;
    protected float _maxHp;
    protected float _damage;
    protected float _range;
    protected float _speed;
    protected float _size;
    protected float _velocityDecrease;
    protected float _knockback;
    protected float _inputKnockBack;
    protected float _collisionBounceKnockback;
    protected bool _canMove;
    protected int _botType;
    protected Sprite[] _botSprites;
    protected Sprite _botSprite;
    protected Vector2 _direction = new Vector2(1, 0);
    public float Hp
    {

        get => _maxHp;
        set => _maxHp = value;
    }
    public float Knockback
    {
        get => _knockback;
        set => _knockback = value;
    }
    public string Tag
    {
        get => _tag;
        set => _tag = value;
    }

    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }

    public float Size
    {
        get => _size;
        set => _size = value;
    }

    public float VelocityDecrease
    {
        get => _velocityDecrease;
        set => _velocityDecrease = value;
    }
    public float Range
    {
        get => _range;
        set => _range = value;
    }

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }
    public float InputKnockBack
    {
        get => _inputKnockBack;
        set => _inputKnockBack = value;
    }
    public float CollisionKnockback
    {
        get => _collisionBounceKnockback;
        set => _collisionBounceKnockback = value;
    }

    public int BotType
    {
        get => _botType; 
        set => _botType = value;
    }
    public Sprite[] BotSprites
    {
        get => _botSprites;
    }

    public Sprite BotSprite
    {
        get => _botSprite;
        set => _botSprite = value;
    }
    public Vector2 Direction 
    { 
        get => _direction; 
        set => _direction = value; 
    }

    private void Awake()
    {
        GetStatsFromScriptableobject();
    }

    public void GetStatsFromScriptableobject()
    {
        _tag = botSettings.Tag;
        _damage = botSettings.Damage;
        _maxHp = botSettings.Hp;
        _range = botSettings.Range;
        _speed = botSettings.Speed;
        _size = botSettings.Size;
        _velocityDecrease = botSettings.VelocityDecrease;
        _knockback = botSettings.Knockback;
        _inputKnockBack = botSettings.InputKnockBack;
        _collisionBounceKnockback = botSettings.CollisionKnockback;
        _canMove = botSettings.Movement;
        _botSprites = botSettings.Botsprites;
    }

   
}