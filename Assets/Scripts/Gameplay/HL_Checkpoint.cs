using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_Checkpoint : MonoBehaviour
{
    public GameObject LocalPlayer = null;
    public GameObject modelLocalPlayer = null;
    public HL_PlayerController localController = null;
    void Start()
    {
        if (LocalPlayer == null)
            LocalPlayer = GameObject.Find("LocalPlayer");

        if (modelLocalPlayer == null)
            modelLocalPlayer = LocalPlayer.transform.Find("PlayerModel").gameObject;
      
        if (localController == null)
            localController = LocalPlayer.GetComponent<HL_PlayerController>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject == modelLocalPlayer)
        {
            localController.SetCheckpointLocation(transform.position);
        }
    }
}
