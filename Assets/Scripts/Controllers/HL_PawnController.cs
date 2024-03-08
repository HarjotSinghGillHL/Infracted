using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class HL_PawnController : MonoBehaviour
{
    public enum EEnemyState : int
    {
        STATE_PATROL = 0,
        STATE_CHASING,
        STATE_SEARCHING,
        STATE_ATTACKING,
        STATE_RETREATING,
        STATE_MAX,
    }

    private NavMeshAgent Agent;

    public GameObject LocalPlayerBase;
    public GameObject LocalPlayerModel;

    public List<GameObject> PatrolPoints;
    int CurrentPatrolPoint = 0;

    private GameObject GameplayObject;

    bool bLocalPlayerInSight = false;
    bool bShouldChaseLocalPlayer = false;

    public EEnemyState InitialState = 0;
    private EEnemyState CurrentState = 0;

    private Vector3 vecOriginalPosition;
    private Vector3 vecLastSeenLocalPlayerPosition;

    static GameObject StateRenderInstance = null;

    HL_UserInterface UIManager = null;
    void Start()
    {
        GameplayObject = GameObject.Find("GameplayObject");
        UIManager = GameplayObject.GetComponent<HL_UserInterface>();

        if (StateRenderInstance == null)
            StateRenderInstance = this.gameObject;

        Agent = GetComponent<NavMeshAgent>();
        vecOriginalPosition = transform.position;
        CurrentState = InitialState;
    }

    float CalculateFov(Vector3 vecTargetPosition,Vector3 vecObserverPosition,Transform Observer)
    {
        Vector3 vecDirection = vecTargetPosition - vecObserverPosition;
        vecDirection.Normalize();

        return Mathf.Acos(Vector3.Dot(vecDirection, Observer.forward)) * Mathf.Rad2Deg;
    }

    bool IsLocalPlayerVisibleToThisTarget()
    {
        Vector3 directionToTarget = LocalPlayerModel.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit))
        {
            if (hit.transform == LocalPlayerModel.transform)
                return true;
        }

        return false;
    }

    bool ShouldChase(float flDistance,float flFovAngle)
    {
        if (flFovAngle < 60.0 && bLocalPlayerInSight)
        {
            return true;
        }

        return false;
    }
    /*
    Vector3 GetRandomDestination(float range)
    {
        Vector3 directionToPlayer = (vecLastSeenLocalPlayerPosition - transform.position).normalized;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * range + directionToPlayer * range;
        randomDirection += transform.position; 

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
    */
    float SearchTimer = 0.0f;
    bool bRotatedOnce = false;
    bool bRotatedTwice = false;
    void Update()
    {

        const float flSearchTime = 5.0f;

        bLocalPlayerInSight = IsLocalPlayerVisibleToThisTarget();

        if (bLocalPlayerInSight)
            vecLastSeenLocalPlayerPosition = LocalPlayerModel.transform.position;

        float flDistance = Mathf.Abs((transform.position - LocalPlayerModel.transform.position).magnitude);
        float flFovAngle = CalculateFov(LocalPlayerModel.transform.position, transform.position, transform);
      
        bShouldChaseLocalPlayer = ShouldChase(flDistance, flFovAngle);


        if (CurrentState != EEnemyState.STATE_SEARCHING)
        {
            SearchTimer = 0.0f;
            bRotatedOnce = false;
            bRotatedTwice = false;
        }

        if (CurrentState != EEnemyState.STATE_PATROL)
            CurrentPatrolPoint = 0;


        switch (CurrentState)
        {
            case EEnemyState.STATE_PATROL:
                {
                    GameObject CurrentPatrolTarget = PatrolPoints[CurrentPatrolPoint];

                    Agent.SetDestination(CurrentPatrolTarget.transform.position);

                    float flPatrolTargetDistance = Mathf.Abs((transform.position - CurrentPatrolTarget.transform.position).magnitude);

                    if (flPatrolTargetDistance < 2.0f)
                    {
                        if (CurrentPatrolPoint + 1 >= PatrolPoints.Count)
                            CurrentPatrolPoint = 0;
                        else
                            CurrentPatrolPoint += 1;
                    }

                    

                    if (bShouldChaseLocalPlayer)
                        CurrentState = EEnemyState.STATE_CHASING;
                    

                    UIManager.State = "PATROL";
                    break;
                }
            case EEnemyState.STATE_CHASING:
                {
                    if (!bLocalPlayerInSight)
                    {
                        CurrentState = EEnemyState.STATE_SEARCHING;
                    }
                    else
                    {
                        if (flDistance > 6.0)
                        {
                            Agent.SetDestination(LocalPlayerModel.transform.position);
                        }
                        else
                        {
                            Agent.SetDestination(transform.position);
                            CurrentState = EEnemyState.STATE_ATTACKING;
                        }
                     
                    }

                    UIManager.State = "CHASING";
                    break;
                }
            case EEnemyState.STATE_SEARCHING:
                {
                    if (bShouldChaseLocalPlayer)
                    {
                        CurrentState = EEnemyState.STATE_CHASING;
                        bRotatedOnce = false;
                        bRotatedTwice = false;
                        SearchTimer = 0.0f;
                    }
                    else
                    {
                        SearchTimer += 1.0f * Time.deltaTime;
                        
                        if (SearchTimer > flSearchTime)
                        {
                            CurrentState = EEnemyState.STATE_RETREATING;
                        }
                        else
                        {
                            if (!bRotatedOnce)
                            {
                                Agent.SetDestination(vecLastSeenLocalPlayerPosition);
                                bRotatedOnce = true;
                            }

                        }
                    }

                    UIManager.State = "SEARCHING (4-5s)";
                    break;
                }
            case EEnemyState.STATE_ATTACKING:
                {
                    if (flDistance > 6.0)
                        CurrentState = EEnemyState.STATE_CHASING;
         
                    UIManager.State = "ATTACKING";
                    break;
                }
            case EEnemyState.STATE_RETREATING:
                {
                    float flRetreatTargetDistance = Mathf.Abs((transform.position - vecOriginalPosition).magnitude);
                   
                    Agent.SetDestination(vecOriginalPosition);

                    if (bShouldChaseLocalPlayer)
                        CurrentState = EEnemyState.STATE_CHASING;
                    else
                    {

                        if (flRetreatTargetDistance < 4.0f)
                            CurrentState = EEnemyState.STATE_PATROL;
                    }

    
                    UIManager.State = "RETREATING";
                    break;
                }
            default:
                {
                    Application.Quit(); //Lmao?
                    break;
                }
        }
    }
}
