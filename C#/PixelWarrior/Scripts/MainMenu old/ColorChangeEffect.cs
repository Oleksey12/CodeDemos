using UnityEngine;
using UnityEngine.UI;

/*
 * Cкрипт управляет переливанием названия игры
 */
public class ColorChangeEffect : MonoBehaviour, IColorChangeEffect
{
    [SerializeField] private Text animatedText;
    private Color _currentColor = new Color();
    private bool _rise = true;

    private void Start()
    {
        _currentColor.a = animatedText.color.a;
        _currentColor.r = 255f;
        _currentColor.g = 80f;
        _currentColor.b = 0f;
    }

    public void ColorChanger() // Создаёт эффект переливания для названия игры
    {
        if (_rise)
        {
            if (_currentColor.g < 180)
            {
                _currentColor.g += 1f;
            }
            else
                _rise = false;
        }
        else
        {
            if (_currentColor.g > 80)
            {
                _currentColor.g -= 1f;
            }
            else
                _rise = true;
        }


        animatedText.color = new Color(_currentColor.r / 255f, _currentColor.g / 255f, _currentColor.b / 255f, 1f);
    }
}