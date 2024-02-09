using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_KillBox : MonoBehaviour
{
    public GameObject modelLocalPlayer = null;
    public GameObject LocalPlayer = null;
    CharacterController localCharacterController = null;

    HL_PlayerController localController;
    void Start()
    {
        if (LocalPlayer == null)
            LocalPlayer = GameObject.Find("LocalPlayer");

        if (modelLocalPlayer == null)
            modelLocalPlayer = LocalPlayer.transform.Find("PlayerModel").gameObject;

        if (modelLocalPlayer != null)
        {
            localCharacterController = modelLocalPlayer.GetComponent<CharacterController>();
            localController = LocalPlayer.GetComponent<HL_PlayerController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == modelLocalPlayer)
            localController.bGoToCheckpoint = true;
    }
    void Update()
    {
        
    }
}
