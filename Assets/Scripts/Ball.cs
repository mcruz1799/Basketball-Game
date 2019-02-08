using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall {
  public IPlayer Owner { get; set; }

  public float Radius { get { return transform.lossyScale.x; } }

  //For use only in BallUserComponent.HoldBall
  public void SetPosition(Vector3 newPosition) {
    transform.localPosition = newPosition;
  }

  //For use only in BallUserComponent.HoldBall
  public void SetParent(Transform newParent) {
    transform.SetParent(newParent, true);
  }
}