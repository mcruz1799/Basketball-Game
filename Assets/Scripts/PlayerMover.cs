using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour, IXzController {
  public float movementSpeed = 1.0f;

  public float X { get { return transform.position.x; } }
  public float Z { get { return transform.position.z; } }

  public float XLook { get { return transform.forward.x; } }
  public float ZLook { get { return transform.forward.z; } }

  public void Move(float xMove, float zMove) {
    Vector3 movementVector = new Vector3(xMove, 0, zMove);
    transform.Translate(movementVector.normalized * movementSpeed);
  }

  public void SetRotation(float xrotation, float zrotation) {
    Vector3 target = new Vector3(-xrotation, 0, zrotation);
    transform.forward = target;
  }
}
