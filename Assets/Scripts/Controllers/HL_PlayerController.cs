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

    Transform camFirstPerson;
    Transform camThirdPerson;
    EPlayerMoveState moveState = EPlayerMoveState.STATE_IDLE;
    EPlayerMoveState moveStateLast = EPlayerMoveState.STATE_IDLE;
    EPlayerMoveState moveStateBeforeJump = EPlayerMoveState.STATE_RUN;

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

    void Start()
    {
        camFirstPerson = transform.Find("FirstPersonCamera");
        camThirdPerson = transform.Find("ThirdPersonCamera");
    }


    void Update()
    {

        float flMouseX = Input.GetAxis("Mouse X");
        float flMouseY = Input.GetAxis("Mouse Y");

        if (flMouseY != 0.0f)
        {
            if (flMouseY < 0.0f)
                camFirstPerson.transform.Rotate(Vector3.right  * Math.Abs(flMouseY));
            else
                camFirstPerson.transform.Rotate(Vector3.left * Math.Abs(flMouseY));
        }

        if (flMouseX != 0.0f)
        {
            if (flMouseX < 0.0f)
                camFirstPerson.transform.Rotate(Vector3.down * Math.Abs(flMouseX));
            else
                camFirstPerson.transform.Rotate(Vector3.up * Math.Abs(flMouseX));
        }

        // this.transform.Rotate(Vector3.up * flMouseX);
    }
}
