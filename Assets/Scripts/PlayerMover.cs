using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{   
    public float movementSpeed = 1.0f;
    private Vector3 movementVector;
    // Start is called before the first frame update
    void Awake()
    {
        movementVector = Vector3.zero;   
    }

    // Update is called once per frame
    void Update()
    {
        movementVector.x = Input.GetAxis("LeftJoystickX") * movementSpeed;
        movementVector.z = -(Input.GetAxis("LeftJoystickY") * movementSpeed);

        transform.Translate(movementVector);
    }
}
