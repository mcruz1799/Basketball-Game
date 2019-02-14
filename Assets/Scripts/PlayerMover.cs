using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerMover : MonoBehaviour, IXzController {
  [SerializeField] private float _initialSpeed;
  public float Speed { get; set; }

  public float X { get { return transform.position.x; } }
  public float Z { get { return transform.position.z; } }

  public float XLook { get { return transform.forward.x; } }
  public float ZLook { get { return transform.forward.z; } }

  private void Awake() {
    Speed = _initialSpeed;
  }

  public void Move(float xMove, float zMove) {
    Vector3 movementVector = new Vector3(xMove, 0, zMove);
    Vector3 oldPosition = transform.position;

    //Moves on world axis, deltatime to smoothen movement
    transform.Translate(movementVector.normalized * Time.deltaTime * Speed, Space.World);
    if (Mathf.Abs(transform.position.x) > GameManager.S.Xboundary || Mathf.Abs(transform.position.z) > GameManager.S.Zboundary) {
      transform.position = oldPosition;
    }
  }

  public void SetRotation(float xLook, float zLook) {
    Vector3 target = new Vector3(xLook, 0, zLook);
    transform.forward = target;
  }

}
