using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public enum EEnemyState : int
{
    STATE_IDLE = 0,
    STATE_PATROL,
    STATE_ALERTED,
    STATE_CHASING,
    STATE_RETREATING,
    STATE_MAX,
}
public class HL_PawnController : MonoBehaviour
{
    // Start is called before the first frame update

    private NavMeshAgent Agent;

    public GameObject LocalPlayerBase;
    public GameObject LocalPlayerModel;

    public EEnemyState InitialState = 0;
    private EEnemyState CurrentState = 0;

    private Vector3 vecOriginalPosition;
    
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        vecOriginalPosition = transform.position;
        CurrentState = InitialState;
    }
    void HandleState()
    {

        switch (CurrentState)
        {
            case EEnemyState.STATE_IDLE:
                {

                    break;
                }
            case EEnemyState.STATE_PATROL:
                {
                    break;
                }
            case EEnemyState.STATE_ALERTED:
                {
                    break;
                }
            case EEnemyState.STATE_CHASING:
                {
                    break;
                }
            case EEnemyState.STATE_RETREATING:
                {
                    break;
                }
            default:
                {
                    Application.Quit(); //Lmao?
                    break;
                }
        }
    }

    float CalculateFov(Vector3 vecTargetPosition,Vector3 vecObserverPosition,Transform Observer)
    {
        Vector3 vecDirection = vecTargetPosition - vecObserverPosition;
        vecDirection.Normalize();

        return Mathf.Acos(Vector3.Dot(vecDirection, Observer.forward)) * Mathf.Rad2Deg;
    }
    void Update()
    {

        float flDistance = Mathf.Abs((transform.position - LocalPlayerModel.transform.position).magnitude);
        float flFovAngle = CalculateFov(LocalPlayerModel.transform.position, transform.position, transform);

        if (flDistance > 6.0 && flFovAngle < 60.0)
            Agent.SetDestination(LocalPlayerModel.transform.position);
        else
            Agent.SetDestination(transform.position);

        HandleState();

    }
}
