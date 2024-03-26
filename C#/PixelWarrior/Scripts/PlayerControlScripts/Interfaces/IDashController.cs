using UnityEngine;
/*
 * Управляет способностью "рывок"
 */
public interface IDashController
{
    public float DashCooldown { get; set; } // Для получения текущего состояния способности другими скриптами
    public bool DashState { get; } // Для определения находится ли игрок вы рывке или нет
    public void DashButton(Vector3 moveVector); // Запуск рывка
    public void RestoreDashCooldown(); // Пассивная перезарядка рывка

    public void DashEffect(); // Эффект создания клонов при рывке
}