using UnityEngine;

/*
 * Интерфейс, описывающий основные параметры бота
 */
public interface IBotCharacteristics
{
    public void GetStatsFromScriptableobject();

    public float Hp { get; set; }
    public float Knockback { get; set; }
    public string Tag { get; set; }
    public float Damage { get; set; }
    public float Size { get; set; }
    public float VelocityDecrease { get; set; }
    public float Range { get; set; }
    public float Speed { get; set; }

    public bool CanMove { get; set; }
    public float InputKnockBack { get; set; }
    public float CollisionKnockback { get; set; }
    public Sprite[] BotSprites {get;}
    public Sprite BotSprite { get; set; }

    public Vector2 Direction { get; set; }
}