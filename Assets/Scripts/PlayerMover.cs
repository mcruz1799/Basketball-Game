using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{   
    public float movementSpeed = 1.0f;
    private Vector3 movementVector;
    public SmallPlayer smallPlayer;
    private bool grounded;
    // Start is called before the first frame update
    void Awake()
    {
        movementVector = Vector3.zero;
        grounded = (smallPlayer.Below == null);   
    }

    // Update is called once per frame
    void Update()
    {
        if (grounded){
            movementVector.x = Input.GetAxis("LeftJoystickX") * movementSpeed;
            movementVector.z = -(Input.GetAxis("LeftJoystickY") * movementSpeed);

            transform.Translate(movementVector);
        }
       
    }
}
