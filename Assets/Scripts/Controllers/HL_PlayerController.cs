using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public enum EPlayerMoveState : int
{
    STATE_IDLE =0,
    STATE_RUN,
    STATE_WALK,
    MOVE_STATE_FORCE_DWORD,
}

public enum EPlayerBodyState : int
{
    BODY_STATE_IDLE = 0,
    BODY_STATE_CROUCH,
    BODY_STATE_JUMP,
    BODY_STATE_PRONE,
    BODY_STATE_FORCE_DWORD,
}

public class HL_PlayerController : MonoBehaviour
{
    HL_KeyState KeyStates;
    Transform camFirstPerson;
    Transform camThirdPerson;
    Transform modelLocalPlayer;
    Transform modelView;
    void Start()
    {
        KeyStates = this.gameObject.GetComponent<HL_KeyState>();
        modelLocalPlayer = transform.Find("PlayerModel");
        camFirstPerson = modelLocalPlayer.Find("FirstPersonCamera");
        camThirdPerson = modelLocalPlayer.Find("ThirdPersonCamera");
        modelView = camFirstPerson.Find("ViewModel");
        quatViewModelInitialPosition = modelView.transform.localRotation;
        InitViewModel();
    }
    void InitViewModel()
    {
    }

    Quaternion quatViewModelInitialPosition = Quaternion.identity;
    EPlayerMoveState moveState = 0;
    EPlayerMoveState moveStateLast = 0;

    EPlayerBodyState bodyState = 0;
    EPlayerBodyState bodyStateLast = 0;

    float flCurrentPlayerSpeed = 0.0f;
    float flVerticalViewModelSway = 0.0f;
    float flHorizontalViewModelSway = 0.0f;

    Vector3 vecMouseMoveDelta = Vector3.zero;
    Vector3 vecKeyboardMoveDelta = Vector3.zero;

    Vector3 vecViewAngles = Vector3.zero;
    float GetPlayerMoveSpeedModifier()
    {
        return 1.0f;
        /*
        if (moveState == (int)EPlayerMoveState.STATE_IDLE)
            return 0.0f;

        float flMoveSpeed = 0.0f;

        switch (moveState)
        {
            case EPlayerMoveState.STATE_RUN:
                {
                    switch (bodyState)
                    {
                        case EPlayerBodyState.BODY_STATE_CROUCH:
                            {
                                flMoveSpeed = 2.5f;
                                break;
                            }

                        case EPlayerBodyState.STATE:
                            {
                                flMoveSpeed = 2.5f;
                                break;
                            }
                        default:
                            break;

                    }
                    flMoveSpeed = 2.5f;
                    break;
                }
  
            case EPlayerMoveState.STATE_WALK:
                {
                    flMoveSpeed = 2.5f;
                    break;
                }
            default:
                break;

        }
        */
       
    }

    float GetPlayerMaxMoveSpeed()
    {
        float flSpeedModifier = GetPlayerMoveSpeedModifier();

            float MaxPlayerSpeed = 5.0f;   //MaxPlayerSpeed will be adjusted according to situations

            return flSpeedModifier * MaxPlayerSpeed;
    }
    public static void ClampAngles(ref Vector3 vecAngles)
    {
        if (vecAngles.x > 89.0f)
            vecAngles.x = 89.0f;
        else if (vecAngles.x < -89.0f)
            vecAngles.x = -89.0f;

        if (vecAngles.y > 180.0f)
            vecAngles.y = 180.0f;
        else if (vecAngles.y < -180.0f)
            vecAngles.y = -180.0f;

    }

    public static void NormalizeAngle(ref float flAngle)
    {
        if (flAngle > 180.0f)
            flAngle = -180.0f;
        else if (flAngle < -180.0f)
            flAngle = 180.0f;
    }
    public static void NormalizeAngles(ref Vector3 vecAngles)
    {
        NormalizeAngle(ref vecAngles.x);
        NormalizeAngle(ref vecAngles.y);

    }

    bool bSprintingKeyState = false;
    bool bCrouchingKeyState = false;
    bool bJumpKeyState = false;
    bool bProneKeyState = false;
    void UpdateMoveState()
    {
        bSprintingKeyState = KeyStates.CheckKeyState(KeyCode.LeftShift, EKeyQueryMode.KEYQUERY_ONHOTKEY);
        bCrouchingKeyState = KeyStates.CheckKeyState(KeyCode.LeftControl, EKeyQueryMode.KEYQUERY_ONHOTKEY);
        bJumpKeyState = KeyStates.CheckKeyState(KeyCode.Space, EKeyQueryMode.KEYQUERY_ONHOTKEY);
        bProneKeyState = KeyStates.CheckKeyState(KeyCode.Z, EKeyQueryMode.KEYQUERY_ONHOTKEY);

        moveStateLast = moveState;
        bodyStateLast = bodyState;

        if (bCrouchingKeyState)
        {
            bodyState = EPlayerBodyState.BODY_STATE_CROUCH;
            bJumpKeyState = false;
            bProneKeyState = false;
        }
        else if (bJumpKeyState)
        {
            bodyState = EPlayerBodyState.BODY_STATE_JUMP;
            bCrouchingKeyState = false;
            bProneKeyState = false;
        }
        else if (bProneKeyState)
        {
            bodyState = EPlayerBodyState.BODY_STATE_PRONE;
            bCrouchingKeyState = false;
            bJumpKeyState = false;
        }
        else
            bodyState = EPlayerBodyState.BODY_STATE_IDLE;

        if (vecKeyboardMoveDelta.y != 0.0f || vecKeyboardMoveDelta.x != 0.0f)
        {
            if (bSprintingKeyState)
                moveState = EPlayerMoveState.STATE_RUN;
            else
                moveState = EPlayerMoveState.STATE_WALK;
        }
        else
        {
            moveState = EPlayerMoveState.STATE_IDLE;
        }

        flCurrentPlayerSpeed = GetPlayerMaxMoveSpeed();

       // Debug.Log("moveState : " + moveState+ " bodyState : " + bodyState + " currentPlayerSPeed : " + flCurrentPlayerSpeed);
    }

    float flSensitivity = 5.0f;

    void Update()
    {
        vecMouseMoveDelta.x = Input.GetAxis("Mouse X") * flSensitivity;
        vecMouseMoveDelta.y = Input.GetAxis("Mouse Y") * flSensitivity;

        vecKeyboardMoveDelta.y = Input.GetAxis("Vertical");
        vecKeyboardMoveDelta.x = Input.GetAxis("Horizontal");
       
        vecViewAngles.x -= vecMouseMoveDelta.y * 1.0f;
        vecViewAngles.y -= vecMouseMoveDelta.x * 1.0f;

        NormalizeAngles(ref vecViewAngles);
        ClampAngles(ref vecViewAngles);

       // transform.localRotation = Quaternion.Euler(0, -vecViewAngles.y, 0);
         camFirstPerson.localRotation = Quaternion.Euler(vecViewAngles.x,0, 0);
         modelLocalPlayer.localRotation = Quaternion.Euler(0, -vecViewAngles.y, 0);

        UpdateMoveState();

        if (vecKeyboardMoveDelta.y != 0)
            modelLocalPlayer.Translate(Vector3.forward * ((vecKeyboardMoveDelta.y * Time.deltaTime)) * flCurrentPlayerSpeed);
       
        if (vecKeyboardMoveDelta.x != 0)
            modelLocalPlayer.Translate(Vector3.right * ((vecKeyboardMoveDelta.x * Time.deltaTime) * flCurrentPlayerSpeed));

        if (vecMouseMoveDelta.x != 0.0f)
        {
            flHorizontalViewModelSway += (vecMouseMoveDelta.x < 0 ? -1.0f : 1.0f) *  ((Time.deltaTime * flSensitivity * 2.0f));
            flHorizontalViewModelSway = Math.Clamp(flHorizontalViewModelSway,-2.0f,2.7f);
        }
        else
        {
            if (flHorizontalViewModelSway < 0.0f)
            {
                flHorizontalViewModelSway += (Time.deltaTime * 3.0f);
            
                if (flHorizontalViewModelSway > 0.0f)
                    flHorizontalViewModelSway = 0.0f;

            }
            else if  (flHorizontalViewModelSway > 0.0f)
            {
                    flHorizontalViewModelSway -= (Time.deltaTime * 3.0f);
              
                if (flHorizontalViewModelSway < 0.0f)
                    flHorizontalViewModelSway = 0.0f;
            }

        }

        if (vecMouseMoveDelta.y != 0.0f)
        {
            flVerticalViewModelSway += (vecMouseMoveDelta.y < 0 ? -1.0f : 1.0f) * ((Time.deltaTime * flSensitivity * 0.2f));
            flVerticalViewModelSway = Math.Clamp(flVerticalViewModelSway, -27.0f, 0.4f);
        }
        else
        {
            if (flVerticalViewModelSway < 0.0f)
            {
                flVerticalViewModelSway += (Time.deltaTime * 0.4f);

                if (flVerticalViewModelSway > 0.0f)
                    flVerticalViewModelSway = 0.0f;

            }
            else if (flVerticalViewModelSway > 0.0f)
            {
                flVerticalViewModelSway -= (Time.deltaTime * 0.4f);

                if (flVerticalViewModelSway < 0.0f)
                    flVerticalViewModelSway = 0.0f;
            }

        }

        Quaternion quatLocRotation = quatViewModelInitialPosition;

        if (flHorizontalViewModelSway != 0.0f)
        {
            quatLocRotation.y += flHorizontalViewModelSway;
          //  quatLocRotation = Quaternion.Euler(0.0f,flHorizontalViewModelSway,0.0f);
        }

        modelView.localRotation = quatLocRotation;

    }
}
