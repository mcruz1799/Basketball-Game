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

  public void HoldBall(IBall ball) {
    ball.SetParent(transform);
    Vector3 ballPosition = Vector3.zero;
    ballPosition.x = transform.lossyScale.x / 2 + ball.Radius;
    ballPosition.z = transform.lossyScale.z / 2 + ball.Radius;
    ball.SetPosition(ballPosition);
  }

  public void Pass(float xDirection, float zDirection) {
    Debug.LogWarning("Pass is not yet implemented");
  }

  public bool Steal(BoxCollider grabHitbox) {
    RaycastHit[] hits = Physics.BoxCastAll(grabHitbox.center, grabHitbox.bounds.extents, Vector3.zero);
    foreach (RaycastHit h in hits) {
      IBall ball = h.collider.GetComponent<Ball>();
      if (ball != null) {
        HoldBall(ball);
        return true;
      }
    }

    return false;
  }
}
