using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To be used by MonoBehaviours implementing IBallUser
[RequireComponent(typeof(PlayerMover))]
public class BallUserComponent : MonoBehaviour {
  [SerializeField] private float heightToHoldBallAt;

  //Needed only for XLook and ZLook
  private IXzController xzController;

  private void Awake() {
    xzController = GetComponent<PlayerMover>();
  }

  public bool HasBall => throw new System.NotImplementedException();

  public void Pass(float xDirection, float zDirection) {
    //HasBall = false;
    throw new System.NotImplementedException();
  }

  public bool Steal(BoxCollider grabHitbox) {
    RaycastHit[] hits = Physics.BoxCastAll(grabHitbox.center, grabHitbox.bounds.extents, Vector3.zero);
    foreach (RaycastHit h in hits) {
      IBall ball = h.collider.GetComponent<Ball>();
      if (ball != null) {
        //ball.SetPosition(new Vector3(???, heightToHoldBallAt, ???));
        //ball.SetParent(transform);
        //return true;
      }
    }

    throw new System.NotImplementedException();
    //return false;
  }
}
