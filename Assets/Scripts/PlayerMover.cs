using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerMover : MonoBehaviour, IXzController {
  public float movementSpeed = 10.0f;

  public float X { get { return transform.position.x; } }
  public float Z { get { return transform.position.z; } }

  public float XLook { get { return transform.forward.x; } }
  public float ZLook { get { return transform.forward.z; } }
  private bool grounded = true;
    // Update is called once per frame
    void Update()
    {   
        if (grounded){
            float Xmove = XCI.GetAxis(XboxAxis.LeftStickX);
            float Zmove = XCI.GetAxis(XboxAxis.LeftStickY);
            Move(Xmove,Zmove);
            //  float Xrotation = Input.GetAxis("RightStickX");
            // float Zrotation = Input.GetAxis("RightStickY");
            float Xrotation = XCI.GetAxis(XboxAxis.RightStickX);
            float Zrotation = XCI.GetAxis(XboxAxis.RightStickY);
            SetRotation(Xrotation,Zrotation);
        }
    }
    

  public void Move(float xMove, float zMove) {
    Vector3 movementVector = new Vector3(xMove, 0, zMove);
    transform.Translate(movementVector.normalized * Time.deltaTime * movementSpeed, Space.World);
    //moves on world axis, deltatime to smoothen movement ^^
  }

  public void SetRotation(float xLook, float zLook) {
    Vector3 target = new Vector3(-xLook, 0, zLook);
    transform.forward = target;
  }
}
