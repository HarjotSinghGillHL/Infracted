using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_ProjectileController : MonoBehaviour
{
    private float speed = 100f; 
    private Vector3 direction;

    CharacterController characterController;

    void Start()
    {
        direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        MoveBullet();
    }
    void MoveBullet()
    {
        Vector3 movement = direction * speed * Time.deltaTime;
        transform.position += movement;
    }

}
