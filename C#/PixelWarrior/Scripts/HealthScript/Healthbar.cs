
using UnityEngine;
using UnityEngine.UI;

/*
 *  Скрипт ответственен за обновление здоровья существа
 */
public class Healthbar : MonoBehaviour, IHealthbar
{

    [SerializeField] private Image mask;
    [SerializeField] private Image form;


    // Обновляет полоску здоровья существа
    public void maskUpdate(float maskFill)
    {
        if(maskFill > 0)
            mask.fillAmount = maskFill;
    }

    // Уничтажает полоску здоровья после смерти существа
    public void DestroyHealthbar()
    {
        Destroy(mask);
        Destroy(form);
    }
}


