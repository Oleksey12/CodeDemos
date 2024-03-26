using UnityEngine;

/*
 ќбъект, определ€ющий данные бота
*/
[CreateAssetMenu(fileName = "NewBot", menuName = "Sciptable")]
public class BotScriptableObject : ScriptableObject
{
    [SerializeField] private string tag;
    [SerializeField] private float maxHp;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float speed;
    [SerializeField] private float size;
    [SerializeField] private float velocityDecrease;
    [SerializeField] private float knockback;
    [SerializeField] private bool haveMovement;

    [SerializeField] private Sprite botType1;
    [SerializeField] private Sprite botType2;
    [SerializeField] private Sprite botType3;
    [SerializeField] private Sprite botType4;
    public float Hp
    {
        get => maxHp;
        set => maxHp = value;
    }
    public float Knockback
    {
        get => knockback;
        set => knockback = value;
    }
    public string Tag
    {
        get => tag;
        set => tag = value;
    }

    public float Damage
    {
        get => damage;
        set => damage = value;
    }
 
    public float Size
    {
        get => size;
        set => size = value;
    }

    public float VelocityDecrease
    {
        get => velocityDecrease;
        set => velocityDecrease = value;
    }
    public float Range
    {
        get => range;
        set => range = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public bool Movement
    {
        get => haveMovement;
        set => haveMovement = value;
    }

    public Sprite BotType1
    {
        get => botType1;
        set => botType1 = value;
    }
    public Sprite BotType2
    {
        get => botType2;
        set => botType2 = value;
    }
    public Sprite BotType3
    {
        get => botType3;
        set => botType3 = value;
    }
    public Sprite BotType4
    {
        get => botType4;
        set => botType4 = value;
    }
}
