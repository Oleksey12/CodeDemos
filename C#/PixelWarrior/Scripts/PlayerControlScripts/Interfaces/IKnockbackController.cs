/*
 * Скрипт управляет эффектом откидывания после нанесения урона
 */

using UnityEngine;

public interface IKnockbackController
{
    public bool isDamaged { get; set; }
    public void SlowDownEffect();
    public void ApplyKnockback(Vector2 knockback);
}

