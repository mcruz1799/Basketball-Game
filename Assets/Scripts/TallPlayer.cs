using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class TallPlayer : Player {
  public SmallPlayer Above { get; private set; }

  public bool ThrowSmallPlayer() {
    Debug.Log("Stuff happens.");
    Debug.Log("Above:" + Above);
    if (Above == null || !Above.OnThrown(XLook, ZLook)) {
      Debug.Log("Throw false.");
      return false;
    }

    Above = null;
    Debug.Log("Throw True.");
    return true;
  }

  public bool PickUpSmallPlayer() {
    //Can't pick up SmallPlayer unless your hands are free and you're not being carried
    bool handsFree = !HasBall && Above == null;
    if (!handsFree) {
      return false;
    }

    //Check grab hitbox
    Vector3 selfToHitbox = grabHitbox.transform.position - transform.position;
    RaycastHit[] hits = Physics.BoxCastAll(transform.position, grabHitbox.bounds.extents, selfToHitbox, Quaternion.identity, selfToHitbox.magnitude);
    foreach (RaycastHit h in hits) {
      SmallPlayer other = h.collider.GetComponent<SmallPlayer>();
      if (other != null && other.Team == Team && other.Below == null) {
        other.OnPickedUp(this);
        Above = other;
        break;
      }
    }

    return true;
  }

  public void OnAboveJumpingOff() {
    Above = null;
  }

  public override void Stun() {
    base.Stun();
    if (Above != null) {
      Above.Stun();
    }
  }

  /*
   Possibilities of Pressing A:
   Tip-Off: Gain control of the ball.
   Pass: If Player has the ball, pass it.
   Pick-Up Small Player: If Player is in range, can pickup the small player.
  */
  public override void PressA(XboxController controller) {
    if (HasBall) {
      Pass();
    } else {
      GameManager.S.CheckTipOff(controller);
      PickUpSmallPlayer();
      
    }
  }
  public override void PressB(XboxController controller) {
    Steal();
    ThrowSmallPlayer();
  }
}
