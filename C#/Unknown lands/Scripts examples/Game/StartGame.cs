using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] protected GameObject loadScreen;
    [SerializeField] protected GameObject menuButton;

    protected AudioSource music;

    protected bool ready = false;
    protected RoomVariants rooms;

    void Start()
    {
        music = GetComponent<AudioSource>();    
        rooms = FindObjectOfType<RoomVariants>();

    }

    private void FixedUpdate() {
        if (!ready && rooms.roomsCount == rooms.roomsLimit) {
            ready = true;
            music.Play();
            menuButton.SetActive(true);
            loadScreen.SetActive(false);
            //playerData.freezeImputs = false;
        }
    }
}
