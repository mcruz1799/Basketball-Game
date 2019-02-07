using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To be used by MonoBehaviours implementing IBallUser
//Doesn't actually implement the IBallUser itself.  This is because it needs additional parameters for Pass/Steal
[RequireComponent(typeof(PlayerMover))]
public class BallUserComponent : MonoBehaviour {
  [SerializeField] private float heightToHoldBallAt;

  //Needed only for XLook and ZLook
  private IXzController xzController;

  private void Awake() {
    xzController = GetComponent<PlayerMover>();
  }

  public bool HasBall => GameManager.S.ball.Owner == this;

  public void Pass(float xDirection, float zDirection) {
    //HasBall = false;
    Debug.LogWarning("Pass is not yet implemented");
  }

  public bool Steal(BoxCollider grabHitbox) {
    RaycastHit[] hits = Physics.BoxCastAll(grabHitbox.center, grabHitbox.bounds.extents, Vector3.zero);
    foreach (RaycastHit h in hits) {
      IBall ball = h.collider.GetComponent<Ball>();
      if (ball != null) {
        //ball.SetPosition(new Vector3(???, heightToHoldBallAt, ???));
        //ball.SetParent(transform);

        Debug.LogWarning("Steal successfully detected the ball, but doesn't do anything with it due to only being partially implemented");
        return true;
      }
    }

    Debug.LogWarning("Steal isn't implemented yet");
    return false;
  }
}
