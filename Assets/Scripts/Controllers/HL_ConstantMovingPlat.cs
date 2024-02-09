using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_ConstantMovingPlat : MonoBehaviour
{
    public GameObject modelLocalPlayer=null;
    public GameObject LocalPlayer = null;
    CharacterController localCharacterController = null;

    private Vector3 vecMaxMove = new Vector3(10, 0, 0);
    private Vector3 vecMinMove = new Vector3(-10, 0, 0);
    private Vector3 vecTargetMoveDelta;
    private Vector3 vecCurrentMoveDelta = new Vector3(0, 0, 0);
    private Vector3 vecVelocity = Vector3.zero;
    private float smoothTime = 0.5f;

    private Vector3 vecInitialPosition;
    private Vector3 lastPosition;
    private bool movingTowardsMax = true;
    private bool playerOnPlatform = false;

    void Start()
    {
        if (LocalPlayer == null)
           LocalPlayer = GameObject.Find("LocalPlayer");
       
        if (modelLocalPlayer == null)
            modelLocalPlayer = LocalPlayer.transform.Find("PlayerModel").gameObject;

        if (modelLocalPlayer != null)
        {
            localCharacterController = modelLocalPlayer.GetComponent<CharacterController>();
        }

        vecInitialPosition = transform.position;
        vecTargetMoveDelta = vecMaxMove;
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == modelLocalPlayer)
        {
            playerOnPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == modelLocalPlayer)
        {
            playerOnPlatform = false;
        }
    }

    void Update()
    {
        Vector3 prevPosition = transform.position;

        vecCurrentMoveDelta = Vector3.SmoothDamp(vecCurrentMoveDelta, vecTargetMoveDelta, ref vecVelocity, smoothTime);
        transform.position = vecInitialPosition + vecCurrentMoveDelta;

        Vector3 platformMovementDelta = transform.position - prevPosition;

        if (playerOnPlatform)
        {
            localCharacterController.Move(platformMovementDelta);
        }

        bool closeToTarget = (vecCurrentMoveDelta - vecTargetMoveDelta).sqrMagnitude < 0.1f * 0.1f;

        if (closeToTarget)
        {
            movingTowardsMax = !movingTowardsMax;
            vecTargetMoveDelta = movingTowardsMax ? vecMaxMove : vecMinMove;
        }
    }
}