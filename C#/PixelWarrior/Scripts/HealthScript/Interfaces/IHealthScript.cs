using UnityEngine;


/*
 * Скрипт определяет функционал скрипта здоровья существ
 */
public abstract class IHealthScript : MonoBehaviour
{
    protected float _max_health;
    protected float _current_health;

    // Получить информацию о здоровье существа
    public virtual float Max_health
    {
        get => _max_health;
    }
    public virtual float Current_health
    {
        get => _current_health;
    }

    // Установить здоровье
    public virtual void setHealth(float curHP, float maxHP) { }
    // Изменить текущее здоровье на
    public virtual void ChangeCurrentHealth(float damage) { }
    
   
    
    // Функция вызвываемая при смерти существа
    protected virtual void Death() { }
}