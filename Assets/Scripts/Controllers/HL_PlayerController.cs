using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerMoveState
{
    STATE_IDLE =0,
    STATE_RUN,
    STATE_WALK,
    STATE_JUMP,
    STATE_PRONE,
    MOVE_STATE_FORCE_DWORD,
}

public class HL_PlayerController : MonoBehaviour
{
    HL_KeyState KeyStates;
    Transform camFirstPerson;
    Transform camThirdPerson;

    EPlayerMoveState moveState = EPlayerMoveState.STATE_IDLE;
    EPlayerMoveState moveStateLast = EPlayerMoveState.STATE_IDLE;
    EPlayerMoveState moveStateBeforeJump = EPlayerMoveState.STATE_RUN;

    float flCurrentPlayerSpeed = 0.0f;

    Vector3 vecMouseMoveDelta = Vector3.zero;
    Vector3 vecKeyboardMoveDelta = Vector3.zero;

    Vector3 vecViewAngles = Vector3.zero;
    float GetPlayerMoveSpeedModifier()
    {
        switch (moveState)
        {
            case EPlayerMoveState.STATE_IDLE:
                return 0.0f;
            case EPlayerMoveState.STATE_RUN:
                return 2.5f;
            case EPlayerMoveState.STATE_WALK:
                return 1.0f;
            case EPlayerMoveState.STATE_JUMP:
                return 0.1f;
            case EPlayerMoveState.STATE_PRONE:
                return 0.3f;
            default:
                return 0.0f;

        }
    }

    float GetPlayerMaxMoveSpeed()
    {
        float flSpeedModifier = GetPlayerMoveSpeedModifier();

            int MaxPlayerSpeed = 230;   //MaxPlayerSpeed will be adjusted according to situations

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
    public static void NormalizeAngles(ref Vector3 vecAngles)
    {
        if (vecAngles.x > 180.0f)
            vecAngles.x = -180.0f;
        else if (vecAngles.x < -180.0f)
            vecAngles.x = 180.0f;

        if (vecAngles.y > 180.0f)
            vecAngles.y = -180.0f;
        else if (vecAngles.y < -180.0f)
            vecAngles.y = 180.0f;
       
    }

    bool bSprintingKeyState = false;
    bool bCrouchingKeyState = false;
    void UpdateMoveState()
    {
        bSprintingKeyState = KeyStates.CheckKeyState(KeyCode.LeftShift, EKeyQueryMode.KEYQUERY_TOGGLE);
        bCrouchingKeyState = KeyStates.CheckKeyState(KeyCode.LeftControl, EKeyQueryMode.KEYQUERY_TOGGLE);
    }
    void Start()
    {
        KeyStates = this.gameObject.GetComponent<HL_KeyState>();
        camFirstPerson = transform.Find("FirstPersonCamera");
        camThirdPerson = transform.Find("ThirdPersonCamera");
    }
    void Update()
    {
        vecMouseMoveDelta.x = Input.GetAxis("Mouse X");
        vecMouseMoveDelta.y = Input.GetAxis("Mouse Y");

        vecKeyboardMoveDelta.y = Input.GetAxis("Horizontal");
        vecKeyboardMoveDelta.x = Input.GetAxis("Vertical");
       
        vecViewAngles.x -= vecMouseMoveDelta.y * 1.0f;
        vecViewAngles.y -= vecMouseMoveDelta.x * 1.0f;

        NormalizeAngles(ref vecViewAngles);
        ClampAngles(ref vecViewAngles);

        camFirstPerson.localRotation = Quaternion.Euler(vecViewAngles.x, -vecViewAngles.y, 0);

        UpdateMoveState();
    }
}
