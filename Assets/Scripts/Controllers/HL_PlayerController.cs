using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
    public GameObject GameplayObject;
    public HL_UserInterface UI;
    public HL_KeyState KeyStates;

    public Transform camFirstPerson;
    public Transform camThirdPerson;
    public Transform modelLocalPlayer;
    public Transform modelView;
    public CharacterController characterController;

    Quaternion quatViewModelInitialPosition = Quaternion.identity;
    EPlayerMoveState moveState = EPlayerMoveState.STATE_IDLE;
    EPlayerBodyState bodyState = EPlayerBodyState.BODY_STATE_IDLE;

    float flCurrentPlayerSpeed = 0.0f;

    Vector3 vecMouseMoveDelta = Vector3.zero;
    Vector3 vecKeyboardMoveDelta = Vector3.zero;
    Vector3 vecViewAngles = Vector3.zero;
    float flSensitivity = 5.0f;

    public float jumpHeight = 2.0f;

    EPlayerMoveState moveStateLast = 0;
    EPlayerBodyState bodyStateLast = 0;

    GameObject AttachedGameObject = null;

    [HideInInspector]
    public bool bGoToCheckpoint = false;

    Vector3 vecCheckpoint = Vector3.zero;
    void OnTriggerEnter(Collider other)
    {
        if (AttachedGameObject.CompareTag("MovingPlane"))
        {
            AttachedGameObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MovingPlane"))
        {
            AttachedGameObject = null;
        }
    }
  
    public void SetCheckpointLocation(Vector3 _vecCheckpoint)
    {
        vecCheckpoint = _vecCheckpoint;
    }
    void RespawnToCheckpoint()
    {
        bGoToCheckpoint = true;
    }
    void Awake()
    {
        //bulletPrefab = Resources.Load<GameObject>("bulletPrefab");
    }
    void Start()
    {
        if (KeyStates == null)
        KeyStates = gameObject.GetComponent<HL_KeyState>();
      
        if (GameplayObject == null)
            GameplayObject = GameObject.Find("GameplayObject");
       
        if (UI == null)
            UI = GameplayObject.GetComponent<HL_UserInterface>();

        if (modelLocalPlayer == null)
            modelLocalPlayer = transform.Find("PlayerModel");
      
        if (camFirstPerson == null)
            camFirstPerson = modelLocalPlayer.Find("FirstPersonCamera");
       
        if (camThirdPerson == null)
            camThirdPerson = modelLocalPlayer.Find("ThirdPersonCamera");
      
        if (modelView == null)
            modelView = camFirstPerson.Find("ViewModel");

        if (characterController == null)
            characterController = modelLocalPlayer.GetComponent<CharacterController>();

        quatViewModelInitialPosition = modelView.transform.localRotation;
        vecCheckpoint = modelLocalPlayer.transform.position;

    }

    void Update()
    {
        if (UI.bMenuPaused)
            return;

        HandleInput();
        UpdateMoveState();
        ApplyMovement();
        HandleFireStage();
    }

    float GetPlayerMoveSpeedModifier()
    {
    
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
                                flMoveSpeed = 1.5f;
                                break;
                            }
                        case EPlayerBodyState.BODY_STATE_PRONE:
                            {
                                flMoveSpeed = 1.0f;
                                break;
                            }
                        default:
                            {
                                flMoveSpeed = 2.5f;
                                break;
                            }

                    }

                    break;
                }
  
            case EPlayerMoveState.STATE_WALK:
                {

                    switch (bodyState)
                    {
                        case EPlayerBodyState.BODY_STATE_CROUCH:
                            {
                                flMoveSpeed = 1.0f;
                                break;
                            }
                        case EPlayerBodyState.BODY_STATE_PRONE:
                            {
                                flMoveSpeed = 0.5f;
                                break;
                            }
                        default:
                            {
                                flMoveSpeed = 1.5f;
                                break;
                            }

                    }

                    break;
                }
            default:
                {
                    flMoveSpeed = 1.5f;
                    break;
                }

        }

        return flMoveSpeed;

      }
    float GetPlayerMaxMoveSpeed()
    {
        float flSpeedModifier = GetPlayerMoveSpeedModifier();

        float MaxPlayerSpeed = 5.0f;   //MaxPlayerSpeed will be adjusted according to situations

        return flSpeedModifier * MaxPlayerSpeed;
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

    void HandleInput()
    {
        vecMouseMoveDelta.x = Input.GetAxisRaw("Mouse X") * flSensitivity;
        vecMouseMoveDelta.y = Input.GetAxisRaw("Mouse Y") * flSensitivity;

        vecKeyboardMoveDelta.y = Input.GetAxisRaw("Vertical");
        vecKeyboardMoveDelta.x = Input.GetAxisRaw("Horizontal");

    }

    private bool isGrounded;

    private float verticalSpeed = 0.0f;
    void ApplyMovement()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && verticalSpeed < 0)
        {
            verticalSpeed = -2f; // A small downward force to ensure the character stays grounded.
        }


        vecViewAngles.x -= vecMouseMoveDelta.y;
        vecViewAngles.y += vecMouseMoveDelta.x;

        vecViewAngles.x = Mathf.Clamp(vecViewAngles.x, -90.0f, 90.0f);

        camFirstPerson.localRotation = Quaternion.Euler(vecViewAngles.x, 0, 0);
        modelLocalPlayer.localRotation = Quaternion.Euler(0, vecViewAngles.y, 0);

        Vector3 moveDirection = modelLocalPlayer.transform.forward * vecKeyboardMoveDelta.y + modelLocalPlayer.transform.right * vecKeyboardMoveDelta.x;
        moveDirection *= flCurrentPlayerSpeed * Time.deltaTime;
      
        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        if (bJumpKeyState && isGrounded)
        {
            verticalSpeed = Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y); 
        }

        moveDirection.y = verticalSpeed * Time.deltaTime; // Apply vertical speed

        characterController.Move(moveDirection);

        if (KeyStates.CheckKeyState(KeyCode.T, EKeyQueryMode.KEYQUERY_SINGLEPRESS))
        {
            TeleportForward();
        }

        if (bGoToCheckpoint)
        {
            characterController.enabled = false;
            modelLocalPlayer.transform.position = vecCheckpoint;

            characterController.enabled = true;

            bGoToCheckpoint = false;
        }
    }


    public float maxTeleportDistance = 5f; 

    public void TeleportAtTargetLocation(Vector3 vecTargetLocation)
    {
        characterController.enabled = false;
        modelLocalPlayer.transform.position = vecTargetLocation;
        characterController.enabled = true;
    }
    void TeleportForward()
    {
        Vector3 startPosition = modelLocalPlayer.transform.position;
        Vector3 direction = modelLocalPlayer.transform.forward;
        float teleportDistance = maxTeleportDistance;

        RaycastHit hit;
        if (Physics.Raycast(startPosition, direction, out hit, maxTeleportDistance))
        {
            teleportDistance = hit.distance - 0.1f;
        }

        Vector3 newPosition = startPosition + direction * teleportDistance;

        characterController.enabled = false;
        modelLocalPlayer.transform.position = newPosition;

        characterController.enabled = true; 
    }

    void HandleFireStage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 spawnPosition = camFirstPerson.transform.position + camFirstPerson.transform.forward;

            Quaternion spawnRotation = camFirstPerson.transform.rotation;

            Instantiate(bulletPrefab, spawnPosition, spawnRotation);

            Debug.Log("Left mouse button clicked.");
        }

    }

    public GameObject bulletPrefab;



}
