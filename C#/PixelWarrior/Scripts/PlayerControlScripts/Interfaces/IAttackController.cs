using UnityEngine;
/*
 * Управляет способностью "атаки"
 */
public interface IAttackController
{
    public float AttackCooldown { get; set; } // Для получения состояния способности другими скриптами
    public void AttackButton(); // Функция кнопки удара

    public void RestoreAttackCooldown(); // Пассивная перезарядка атаки

}