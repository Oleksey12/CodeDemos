using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
/*
 * Реализует эффект перемещения облаков в главном меню
 */

public class CloudManagerUpgraded : MonoBehaviour, ICloudManager
{
    [SerializeField] private GameObject[] _clouds;
    // Хешируем позиции облаков для осуществления их передвижения
    private List<UnityEngine.Transform> _positions;

    private const int ScreenSizeX = 1280;
    private const int ScreenSizeY = 720;
    private void Start()
    {
        _positions = new List<UnityEngine.Transform>();
        foreach (var cloud in _clouds)
            _positions.Add(cloud.transform);
    }

    private void Update()
    {
        CloudController();
    }


    public void CloudController()
    {
        // Для каждого облака на экране
        for (int i = 0; i < _positions.Count; ++i)
        {
            // Если облако вылетело за границы экрана
            if (_positions[i].localPosition.x > 1.2f * ScreenSizeX/2)
                ReCreateCloud(i);
            else // Иначе просто передвигаем облако
                _positions[i].localPosition += new Vector3(30*Time.deltaTime,0,0);
        }
    }

    public void ReCreateCloud(int cloudNum)
    {
        // Фукнция перемещает обалко в начало движения (левую часть экрана)
        _positions[cloudNum].localPosition = new Vector3(-1.2f*ScreenSizeX/2, Random.Range(0.1f * ScreenSizeY/2, 0.9f * ScreenSizeY/2), _positions[cloudNum].position.z);
        float newSize = Random.Range(0.8f, 1.3f);
        _positions[cloudNum].localScale = new Vector3(newSize, newSize, newSize);
    }
}

