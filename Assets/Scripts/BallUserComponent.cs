using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMover))]
public class BallUserComponent : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private float localHeightToHoldBallAt;
#pragma warning restore 0649

  private IXzController xzController; //Needed only for XLook and ZLook

  private IBall heldBall;

  private void Awake() {
    xzController = GetComponent<PlayerMover>();
  }

  public bool HasBall { get { return heldBall != null; } }

  public void HoldBall(IBall ball) {
    heldBall = ball;
    if (ball != null) {
      ball.SetParent(transform);
      Vector3 ballPosition = Vector3.zero;
      ballPosition.y = localHeightToHoldBallAt;
      ballPosition.z = transform.lossyScale.z / 2 + ball.Radius;
      ball.SetPosition(ballPosition);
    }
  }

  public bool Steal(BoxCollider grabHitbox) {
    Vector3 selfToHitbox = grabHitbox.center - transform.position;
    RaycastHit[] hits = Physics.BoxCastAll(transform.position, grabHitbox.bounds.extents, selfToHitbox, Quaternion.identity, selfToHitbox.magnitude);
    foreach (RaycastHit h in hits) {
      BallUserComponent other = h.collider.GetComponent<BallUserComponent>();
      if (other.heldBall != null) {
        HoldBall(other.heldBall);
        other.heldBall = null;
      }
    }

    return false;
  }

  public void Pass() {
    Vector3 PassDirection = new Vector3(xzController.XLook, 0, xzController.ZLook);
    Debug.LogWarning("Pass is not yet implemented");
  }
}
