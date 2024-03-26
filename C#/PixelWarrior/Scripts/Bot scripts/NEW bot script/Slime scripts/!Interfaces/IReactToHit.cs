using UnityEngine;


/*
 * Скрипт определяет действия бота при контакте с хитбоксом атаки
 */
public interface IReactToHit
{
    public void ReactToHit(Vector3 senderPosition);
}


