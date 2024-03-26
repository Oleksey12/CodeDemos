/*
 * Интерфейс определяет перемещение бабочки по уже заготовленным точкам
 */
using System.Collections;
using UnityEngine;
public interface IVisitRandomPoint
{
    public bool MoveToCurrentPoint();

    public void setNewPoint();

}