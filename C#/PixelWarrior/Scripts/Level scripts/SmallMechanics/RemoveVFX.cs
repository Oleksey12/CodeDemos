using UnityEngine;

/*
 * Скрипт управляте отоборажения эффектов (для оптимизации игры).
 */
public class RemoveVFX : MonoBehaviour
{
    [SerializeField] private GameObject[] _vfxElements;
    private BaseValues _baseValues;


    void Start()
    {
        _baseValues = GetComponent<BaseValues>();

        if (_baseValues.ShowVFX == 0)
            HideAllVFX();
    }

    private void HideAllVFX()
    {
        for (int i = 0; i < _vfxElements.Length; i++)
        {
            _vfxElements[i].SetActive(false);
        }
    }

}
