using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallUserComponent : MonoBehaviour, IBallUser {
  public bool HasBall => throw new System.NotImplementedException();

  public void Pass() {
    throw new System.NotImplementedException();
  }

  public void Steal() {
    throw new System.NotImplementedException();
  }
}
