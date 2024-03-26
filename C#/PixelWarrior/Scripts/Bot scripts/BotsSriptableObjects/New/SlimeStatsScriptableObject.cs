using UnityEngine;

/*
 ќбъект, определ€ющий данные бота
*/
[CreateAssetMenu(fileName = "NewBot", menuName = "SlimeScriptableObject")]
public class SlimeStatsScriptableObject : ScriptableObject
{
    [SerializeField] protected string tag;
    [SerializeField] protected float maxHp;
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float speed;
    [SerializeField] protected float size;
    [SerializeField] protected float velocityDecrease;
    [SerializeField] protected float knockback;
    [SerializeField] protected float inputKnockBack;
    [SerializeField] protected float collisionBounceKnockback;
    [SerializeField] protected bool canMove;

    [SerializeField] private Sprite[] botSprites;

    public float Hp
    {
        get => maxHp;
    }
    public float Knockback
    {
        get => knockback;
    }
    public string Tag
    {
        get => tag;
    }

    public float Damage
    {
        get => damage;
    }

    public float Size
    {
        get => size;
    }

    public float VelocityDecrease
    {
        get => velocityDecrease;
    }
    public float Range
    {
        get => range;
    }

    public float Speed
    {
        get => speed;
    }

    public bool Movement
    {
        get => canMove;
    }
    public float InputKnockBack
    {
        get => inputKnockBack;
    }
    public float CollisionKnockback
    {
        get => collisionBounceKnockback;
    }
    public Sprite[] Botsprites
    {
        get => botSprites;
    }

}
