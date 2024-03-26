using UnityEngine;


/*
 * Интерфейс объединяет функционал бота.
 * 
 * Бот обязательно должен:
 * 1) Устанавливать параметры из своего Scriptable объекта
 * 2) Выполнять функции поведения (двигаться/аттаковать/умирать)
 * 3) Иметь очки здоровья
 * 4) Обрабатывать столкновения (с игроком/ объектами / другими ботами)
 */
public abstract class IBotAI : MonoBehaviour
{
    protected IBotCharacteristics _characteristics;
    protected IHealthScript _botHealth;


    protected virtual void HashAllSubscripts()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _botHealth = GetComponent<IHealthScript>();
    }

    protected virtual void Start()
    {
        HashAllSubscripts();
    }


    protected virtual void SetStats()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

}
