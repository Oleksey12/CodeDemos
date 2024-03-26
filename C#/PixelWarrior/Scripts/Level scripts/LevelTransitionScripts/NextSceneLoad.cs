using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Скрипт управляет телепортом в следующую сцену
 */
public class NextSceneLoad : MonoBehaviour, INextSceneLoad
{

    [SerializeField] string _nextSceneName;
    public void GoToNextScene()
    {
        SaveData();
        SceneManager.LoadScene(_nextSceneName);
    }
    private void SaveData()
    {
        FindObjectOfType<PlayerData>().CurrentHealth = FindObjectOfType<HealthScript>().Current_health;
        FindObjectOfType<PlayerData>().SaveStartParams();
    }
}