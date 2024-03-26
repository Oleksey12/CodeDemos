using UnityEngine;

internal interface IMoveController
{
    public Vector2 MoveVector { get; }
    public void MoveVectorCount(); // Cчитываем данные с джойстика
    public void PlayerMovementController(); // Управляет передвижением игрока и анимациями

}