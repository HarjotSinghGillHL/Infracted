using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_UserInterface : MonoBehaviour
{
    // Start is called before the first frame update
    HL_KeyState KeyStates;

    [HideInInspector]
    public bool bMenuPaused = false;
   
    [HideInInspector]
    public float flTimeScaleToAdjust = 0.0f;
    void Start()
    {
        KeyStates = gameObject.GetComponent<HL_KeyState>();
        flTimeScaleToAdjust = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
     if (KeyStates.CheckKeyState(KeyCode.Escape,EKeyQueryMode.KEYQUERY_SINGLEPRESS))
        {
            bMenuPaused = !bMenuPaused;
            if (bMenuPaused) {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = flTimeScaleToAdjust;
            }

        }
    }

    void OnGUI()
    {
        if (bMenuPaused)
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
            guiStyle.fontSize = 60; 
            guiStyle.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 300, 100);
            GUI.Label(rect, "PAUSED", guiStyle);
        }
    }

}
