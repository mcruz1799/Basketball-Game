using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To be used by MonoBehaviours implementing IBallUser
public class BallUserComponent : MonoBehaviour {
  [SerializeField] private float heightToHoldBallAt;

  public bool HasBall => throw new System.NotImplementedException();

  public void Pass(float xDirection, float zDirection) {
    //HasBall = false;
    throw new System.NotImplementedException();
  }

  public void Steal(Collider hitbox) {
    throw new System.NotImplementedException();
  }
}
