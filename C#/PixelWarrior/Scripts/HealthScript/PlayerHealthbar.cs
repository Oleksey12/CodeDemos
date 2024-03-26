
using UnityEngine;
using UnityEngine.UI;

/*
 *  Скрипт ответственен за обновление здоровья игрока
 */
public class PlayerHealthbar : MonoBehaviour, IHealthbar
{
    private Image mask;

    private void Awake()
    {
        // Находим и хешируем объект маски игрока для последующего обновления
        mask = GameObject.FindGameObjectWithTag("HealthForm").GetComponent<Image>();
    }
    public void maskUpdate(float maskFill)
    {
        if (maskFill > 0)
            mask.fillAmount = maskFill;
    }

    public void DestroyHealthbar()
    {
        Destroy(mask);
    }
}