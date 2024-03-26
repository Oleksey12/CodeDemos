/*
 * Скрипт определяет поведение хитбокса игрока
 */

using Unity.VisualScripting;
using UnityEngine;
public interface IDamageBoxBehaviour
{

    public void AffectTarget(Collider2D target);

}


