using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Объект с параметрами карты
*/
[CreateAssetMenu(fileName = "NewCard", menuName = "New _card")]
public class CardScriptableObject : ScriptableObject
{
    [SerializeField] private Sprite card;
    [SerializeField] private float hp = 1;
    [SerializeField] private float maxHp = 1;
    [SerializeField] private float dmg = 1;
    [SerializeField] private float dash = 1;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private float dmg_cooldown = 1;
    [SerializeField] private float speed = 1;


    public Sprite Card { get => card; set => card = value; }
    public float Hp { get => hp; set => hp = value; }
    public float MaxHp { get => maxHp; set => maxHp = value; }
    public float Dmg { get => dmg; set => dmg = value; }
    public float Dash { get => dash; set => dash = value; }
    public float Cooldown { get => cooldown; set => cooldown = value; }
    public float Dmg_cooldown { get => dmg_cooldown; set => dmg_cooldown = value; }
    public float Speed { get => speed; set => speed = value; }

}
