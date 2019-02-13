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

  private bool outOfBounds = false;
    

      public void Move(float xMove, float zMove) {
        if (!outOfBounds)
        {
          Vector3 movementVector = new Vector3(xMove, 0, zMove);

          //Moves on world axis, deltatime to smoothen movement
          transform.Translate(movementVector.normalized * Time.deltaTime * movementSpeed, Space.World);
        }
  }

  public void SetRotation(float xLook, float zLook) {
    Vector3 target = new Vector3(xLook, 0, zLook);
    transform.forward = target;
  }
  private void OnTriggerEnter(Collider other) {
    Debug.Log("collision");
    if (other.gameObject.CompareTag("boundary"))
    {
      Debug.Log("HELLZ YEAH");
      outOfBounds = true;
    }
  }
  
  private void OnTriggerExit(Collider other) {
   if (other.gameObject.CompareTag("boundary"))
    {
      outOfBounds = false;
    }
  }     
}
