using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HL_OnTriggerForDoor : MonoBehaviour
{
    GameObject modelLocalPlayer;
    GameObject LocalPlayer;

    GameObject DoorArea;
    GameObject MainDoor;

    HL_DoorHandler DoorHandler;

    // Start is called before the first frame update
    void Start()
    {
        LocalPlayer = GameObject.Find("LocalPlayer");
        modelLocalPlayer = LocalPlayer.transform.Find("PlayerModel").gameObject;

        DoorArea = GameObject.Find("TriggerDoorArea");
        MainDoor = DoorArea.transform.Find("MainDoor").gameObject;

        DoorHandler = MainDoor.GetComponent<HL_DoorHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == modelLocalPlayer)
        {
            DoorHandler.bOpenDoor = true;
            Debug.Log("Entered Trigger Local");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == modelLocalPlayer)
        {
            DoorHandler.bOpenDoor = false;
            Debug.Log("Exit Trigger Local");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
