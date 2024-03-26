using UnityEngine;

/*
 * Управляет отображением хп существа на экране
 */
public interface IHealthbar
{
    public void maskUpdate(float maskFill);

    public void DestroyHealthbar();
}