using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * C����� ��������� ���� ����� ����������� �������
 */
public class StartAgain : MonoBehaviour
{
    public void LoadStartScene() => SceneManager.LoadScene("NewMainMenu");
}
