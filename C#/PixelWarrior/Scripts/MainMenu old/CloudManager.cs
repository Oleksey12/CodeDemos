using UnityEngine;
/*
 * Управляет поведением облаков в меню
 */
public class CloudManager : MonoBehaviour, ICloudManager
{
    [SerializeField] private GameObject cloud;
    [SerializeField] private Canvas parentCanv;
    [SerializeField] private int _cloudCount = 5;

    private const float _minIncrease = 1.1f, _maxIncrease = 1.8f;
    private Vector3[] _coords;
    private GameObject[] _clouds;


    private void Start()
    {
        // Выделяем память в массивах, хранящих данные об облаках
        _coords = new Vector3[_cloudCount];
        _clouds = new GameObject[_cloudCount];
        // Создаём n облак на экране
        SpawnClouds(false);
    }

    public void CloudController() // Управляет передвижением облак
    {
        // Для каждого облака на экране
        for (int i = 0; i < _coords.Length; ++i)
        {

            // Если облако вылетело за границы экрана
            if (_clouds[i].transform.position.x > Screen.width * 1.2f)
            {
                Destroy(_clouds[i]);
                ReCreateCloud(i);
            }
            else // Иначе просто передвигаем облако
            {
                _coords[i].x += 2;
                _clouds[i].transform.position = _coords[i];
            }
        }
    }
    public void ReCreateCloud(int cloudNum) // Пересозадёт облако при выходе его из области видимости
    {
        _coords[cloudNum] = new Vector3(-0.2f * Screen.width, Random.Range(0.6f * Screen.height, 0.94f * Screen.height), -1f);
        _clouds[cloudNum] = Instantiate(cloud);
        _clouds[cloudNum].transform.position = _coords[cloudNum];
        _clouds[cloudNum].transform.SetParent(parentCanv.transform);
        float increase = Random.Range(_minIncrease, _maxIncrease);
        _clouds[cloudNum].transform.localScale *= increase;

    }
    public void SpawnClouds(bool respawn) // Создаёт все облака
    {
        for (int i = 0; i < _coords.Length; ++i)
        {
            if (respawn)
                Destroy(_clouds[i]);
            _coords[i] = new Vector3(Random.Range(-0.2f * Screen.width, 1.2f * Screen.width), Random.Range(0.6f * Screen.height, 0.94f * Screen.height), -1f);
            _clouds[i] = Instantiate(cloud);
            _clouds[i].transform.position = _coords[i];
            _clouds[i].transform.SetParent(parentCanv.transform);
            float increase = Random.Range(_minIncrease, _maxIncrease);
            _clouds[i].transform.localScale *= increase;
        }
    }
}