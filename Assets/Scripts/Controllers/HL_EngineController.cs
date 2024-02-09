using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_EngineController : MonoBehaviour
{
    HL_KeyState KeyStates;
    HL_UserInterface UI;
    // Start is called before the first frame update
    float flOldTimeScale = 0;
    void Start()
    {
        UI = gameObject.GetComponent<HL_UserInterface>();
        KeyStates = gameObject.GetComponent<HL_KeyState>();
        flOldTimeScale = Time.timeScale;

    }

    // Update is called once per frame
    void Update()
    {
        if (KeyStates.CheckKeyState(KeyCode.P, EKeyQueryMode.KEYQUERY_SINGLEPRESS))
        {
            UI.bNoPhysics = !UI.bNoPhysics;

            if (UI.bNoPhysics)
            {
                Time.timeScale = 0f; // Set fixedDeltaTime to zero to effectively stop physics simulation
            }
            else
            {
                Time.timeScale = flOldTimeScale; // Restore fixedDeltaTime to its original value to resume physics simulation
            }

        }

        if (KeyStates.CheckKeyState(KeyCode.G, EKeyQueryMode.KEYQUERY_SINGLEPRESS))
        {
            UI.bNoGravity = !UI.bNoGravity;

            if (UI.bNoGravity)
            {
                Physics.gravity = Vector3.zero; 
            }
            else
            {
                Physics.gravity = new Vector3(0, -9.81f, 0); 
            }

        }

    }
}
