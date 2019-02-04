using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

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
            //  float Xrotation = Input.GetAxis("RightStickX");
            // float Zrotation = Input.GetAxis("RightStickY");
            float Xrotation = XCI.GetAxis(XboxAxis.RightStickX);
            float Zrotation = XCI.GetAxis(XboxAxis.RightStickY);
            SetRotation(Xrotation,Zrotation);
        }
    }
    void Move()
    {
        //movementVector.x = Input.GetAxis("LeftStickX") * movementSpeed;
        //movementVector.z = -(Input.GetAxis("LeftStickY") * movementSpeed);
        movementVector.x = XCI.GetAxis(XboxAxis.LeftStickX) * movementSpeed;
        movementVector.z = -(XCI.GetAxis(XboxAxis.LeftStickY) * movementSpeed);
        transform.Translate(movementVector);
    }

    void SetRotation(float xrotation, float zrotation)
    {
        Vector3 target = new Vector3(-xrotation, 0, zrotation);
        transform.forward = target;
    }
}
