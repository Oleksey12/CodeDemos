using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * Скрипт управляет отображением шейдров в игре
 */
public class RemoveShaders : MonoBehaviour
{
    [SerializeField] private GameObject[] _shaderElements;
    [SerializeField] private Material _default;
    private BaseValues _baseValues;

    private void Start()
    {
        _baseValues = GetComponent<BaseValues>();

        if (_baseValues.ShowVFX == 0)
            HideAllShaders();
    }

    private void HideAllShaders()
    {
        for (int i = 0; i < _shaderElements.Length; i++)
        {
            _shaderElements[i].GetComponent<TilemapRenderer>().material = _default;
        }
    }
}
