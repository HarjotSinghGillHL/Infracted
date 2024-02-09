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

    GUIStyle guiStylePaused = null;
    GUIStyle guiStyleIndicators = null;
 
    [HideInInspector]
    public int Score = 0;
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
        if (guiStylePaused == null)
        {
            guiStylePaused = new GUIStyle(GUI.skin.label);
            guiStylePaused.fontSize = 60;
            guiStylePaused.alignment = TextAnchor.MiddleCenter;

        }

        if (guiStyleIndicators == null)
        {
            guiStyleIndicators = new GUIStyle(GUI.skin.label);
            guiStyleIndicators.fontSize = 20;
        }

        if (bMenuPaused)
        {
            Rect rect_ = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 300, 100);
            GUI.Label(rect_, "PAUSED", guiStylePaused);
        }

        int Pad = 10;

        Rect rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Movement keys : WASD", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Jump : Space", guiStyleIndicators);
       
        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Fire : LMB", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Teleport ability : T", guiStyleIndicators);
        
        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Pause : Escape", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Triggers : Transparent Purple", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Killboxes : Transparent dark red", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Checkpoints : Transparent yellow", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Collectables : Transparent blue", guiStyleIndicators);
      
        Pad += 30;
        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Score : " + Score, guiStyleIndicators);
    }

}
