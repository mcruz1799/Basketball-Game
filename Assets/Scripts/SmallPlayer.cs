using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallUserComponent))]
public class SmallPlayer : MonoBehaviour, IBallUser {
  private IBallUser ballUserComponent;

  public SmallPlayer Above { get; private set; }
  public SmallPlayer Below { get; private set; }

  private void Awake() {
    ballUserComponent = GetComponent<BallUserComponent>();
  }

  private bool JumpOffPlayer() {

    //Can't jump off if you're not being carried
    if (Below == null) {
      return false;
    }

    return true;
  }

  private bool PickUpPlayer() {

    //Can't pick up a another SmallPlayer unless your hands are free and you're not being carried
    if (Above != null || Below != null || HasBall) {
      return false;
    }

    SmallPlayer[] smallPlayersInFrontOf;
    smallPlayersInFrontOf = null; //TODO: Implement this

    foreach (SmallPlayer other in smallPlayersInFrontOf) {
      if (other.Below == null) {
        other.Below = this;
        Above = other;

        //TODO: Adjust position of other

        break;
      }
    }

    return true;
  }


  //
  //IBallUser
  //

  public bool HasBall => ballUserComponent.HasBall;

  public void Pass() {
    ballUserComponent.Pass();
  }

  public void Steal() {
    ballUserComponent.Steal();
  }
}
