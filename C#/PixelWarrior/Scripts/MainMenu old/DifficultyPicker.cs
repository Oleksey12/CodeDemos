using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;
/*
 * ������, ������������� �� ������� ������ ��������� ����.
 */
public class DifficultyPicker : MonoBehaviour
{
    // ������ ���� ������ ���������:
    public void EasyDifficulty()
    {
        float difficulty = 1.0f;
        FindObjectOfType<BaseValues>().Difficulty = difficulty;
        StartGame();
    }
    public void NormalDifficulty()
    {
        float difficulty = 1.1f;
        FindObjectOfType<BaseValues>().Difficulty = difficulty;
        StartGame();
    }
    public void HardDifficulty()
    {
        float difficulty = 1.2f;
        FindObjectOfType<BaseValues>().Difficulty = difficulty;
        StartGame();
    }
    public void InsaneDifficulty()
    {
        float difficulty = 1.4f;
        FindObjectOfType<BaseValues>().Difficulty = difficulty;
        StartGame();
    }
    public void ExitButton()
    {
        ToggleCanvasOn(GameObject.FindGameObjectWithTag("UICanv").GetComponent<Canvas>());
        Destroy(gameObject);
    }


    // ��������������� �������
    private void ToggleCanvasOn(Canvas canv)
    {
        canv.enabled = true;
    }
    private void StartGame()
    {
        FindObjectOfType<BaseValues>().SaveStartParams();
        FindObjectOfType<PlayerData>().SaveStartParams();
        SceneManager.LoadScene("SampleScene");
    }
    


}
