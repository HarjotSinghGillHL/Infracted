using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class HL_Collectable : MonoBehaviour
{
    public GameObject LocalPlayer = null;
    public GameObject modelLocalPlayer = null;

    public HL_PlayerController localController = null;

    public AudioSource audioSource = null;

    GameObject GameplayObject;
    HL_UserInterface UI;
    void Start()
    {

        if (LocalPlayer == null)
            LocalPlayer = GameObject.Find("LocalPlayer");

        if (modelLocalPlayer == null)
            modelLocalPlayer = LocalPlayer.transform.Find("PlayerModel").gameObject;
        
        if (localController == null)
            localController = LocalPlayer.GetComponent<HL_PlayerController>();

        GameplayObject = GameObject.Find("GameplayObject");
        UI = GameplayObject.GetComponent<HL_UserInterface>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject == modelLocalPlayer)
        {
            UI.Score += 1;
            audioSource.Play();
            GameObject.Destroy(gameObject);
        }
    }
}
