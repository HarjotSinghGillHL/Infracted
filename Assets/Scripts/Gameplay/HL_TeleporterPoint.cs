using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HL_TeleporterPoint : MonoBehaviour
{
    public GameObject TargetTeleportObject;
    public GameObject LocalPlayerObject;
    private HL_PlayerController Controller;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        Controller = LocalPlayerObject.GetComponent<HL_PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Controller.TeleportAtTargetLocation(TargetTeleportObject.transform.position);
        audioSource.Play();
    }

    void Update()
    {
        
    }


}
