using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HL_OnTriggerForDoor : MonoBehaviour
{
    public GameObject modelLocalPlayer;
    public GameObject LocalPlayer;

    public GameObject DoorArea;
    public GameObject MainDoor;

    HL_DoorHandler DoorHandler;

    // Start is called before the first frame update
    void Start()
    {
        if (LocalPlayer == null)
            LocalPlayer = GameObject.Find("LocalPlayer");
       
        if (modelLocalPlayer == null)
            modelLocalPlayer = LocalPlayer.transform.Find("PlayerModel").gameObject;

        if (DoorArea == null)
           DoorArea = GameObject.Find("TriggerDoorArea");
       
        if (MainDoor == null)
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
