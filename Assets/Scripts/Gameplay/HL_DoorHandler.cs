using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_DoorHandler : MonoBehaviour
{
    // Start is called before the first frame update

    [HideInInspector]
    public bool bOpenDoor = false;

    private float UnitsToMoveDoor = 0.0f;
    private float UnitsMoved = 0;

    private Vector3 vecInitialPosition = Vector3.zero;
    void Start()
    {
        vecInitialPosition = transform.position;
        UnitsToMoveDoor = transform.localScale.z;

        vecInitialPosition.z -= UnitsToMoveDoor;

        UnitsMoved = UnitsToMoveDoor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bOpenDoor)
        {
            if (UnitsMoved < 30)
            {

                UnitsMoved += 10.0f * Time.deltaTime;
                UnitsMoved = Mathf.Clamp(UnitsMoved, 0.0F, UnitsToMoveDoor);
            }
        }
        else
        {
            if (UnitsMoved > 0)
            {
                UnitsMoved -= 10.0f * Time.deltaTime;
                UnitsMoved = Mathf.Clamp(UnitsMoved, 0.0F, UnitsToMoveDoor);
            }
        }
        transform.position = new Vector3(vecInitialPosition.x, vecInitialPosition.y, vecInitialPosition.z + UnitsMoved);
    }
}
