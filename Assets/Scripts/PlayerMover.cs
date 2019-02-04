using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{   
    public float movementSpeed = 1.0f;
    private Vector3 movementVector;
    private Vector3 rotationVector;
    public SmallPlayer smallPlayer;
    private bool grounded = true;
    private float timeCount = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {
        movementVector = Vector3.zero;
        rotationVector = Vector3.zero;
        //grounded = (smallPlayer.Below == null);   
    }

    // Update is called once per frame
    void Update()
    {   
        if (grounded){
            Move();
            float Xrotation = Input.GetAxis("RightJoystickX");
            float Zrotation = Input.GetAxis("RightJoystickY");
            SetRotation(Xrotation,Zrotation);
        }
    }
    void Move()
    {
        movementVector.x = Input.GetAxis("LeftJoystickX") * movementSpeed;
        movementVector.z = -(Input.GetAxis("LeftJoystickY") * movementSpeed);

        transform.Translate(movementVector);
    }

    void SetRotation(float xrotation, float zrotation)
    {
        Vector3 target = new Vector3(-xrotation, 0, zrotation);
        transform.forward = target;
    }
}
