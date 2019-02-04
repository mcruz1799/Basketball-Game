using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{   
    public float movementSpeed = 1.0f;
    public SmallPlayer smallPlayer;
    private bool grounded = true;
    // Start is called before the first frame update
    void Awake()
    {
        //grounded = (smallPlayer.Below == null);   
    }

    // Update is called once per frame
    void Update()
    {   
        if (grounded)
        {
            float xMove = Input.GetAxis("LeftJoystickX") * movementSpeed;
            float zMove = -(Input.GetAxis("LeftJoystickY") * movementSpeed);
            Move(xMove,zMove);
            float Xrotation = Input.GetAxis("RightJoystickX");
            float Zrotation = Input.GetAxis("RightJoystickY");
            SetRotation(Xrotation,Zrotation);
        }
    }
    void Move(float xmove, float zmove)
    {
        Vector3 movementVector = new Vector3(xmove,0,zmove);
        transform.Translate(movementVector);
    }

    void SetRotation(float xrotation, float zrotation)
    {
        Vector3 target = new Vector3(-xrotation, 0, zrotation);
        transform.forward = target;
    }
}
