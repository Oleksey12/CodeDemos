using UnityEngine;


/*
 * Скрипт управляет телепортом на следующий уровень
 */
public class NextSceneTeleport : MonoBehaviour
{
    INextSceneLoad nextScene;
    private void Start()
    {
        nextScene = GetComponent<INextSceneLoad>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            nextScene.GoToNextScene();
    }

}
