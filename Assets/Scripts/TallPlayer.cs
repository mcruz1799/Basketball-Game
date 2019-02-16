using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class TallPlayer : Player {

#pragma warning disable 0649
  //DO NOT REFERENCE THESE DIRECTLY.
  //USE THE ISpriteAnimator FIELDS INSTEAD
  [SerializeField] private SpriteAnimator _tpIdle;
  [SerializeField] private SpriteAnimator _tpRun;
#pragma warning restore 0649

  private ISpriteAnimator idleAnimation;
  private ISpriteAnimator runAnimation;

  public SmallPlayer Above { get; private set; }

  public override float Speed {
    get {
      float carryPenalty = Above == null ? 1f : 0.5f;
      return base.Speed * carryPenalty;
    }
  }

  protected override void Awake() {
    base.Awake();
    idleAnimation = _tpIdle;
    runAnimation = _tpRun;
  }

  public bool ThrowSmallPlayer() {
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
      Debug.LogFormat("Failed to pick SmallPlayer up.  HasBall: {0}   Above != null: {1}", HasBall, Above != null);
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
  public override void AButtonDown(XboxController controller) {
    if (HasBall) {
      Pass();
    } else {
      GameManager.S.CheckTipOff(controller);
      PickUpSmallPlayer();
    }
  }
  public override void RTButtonDown(XboxController controller) {
    StartDashing();
  }
  public override void RTButtonUp(XboxController controller) {
    StopDashing();
  }
  public override void BButtonDown(XboxController controller) {
    if (Above == null) {
      Steal();
    } else {
      ThrowSmallPlayer();
    }
  }
  public override void XButtonDown(XboxController controller) {
    Stun();
  }
  public override void Move(float xMove, float zMove) {

    if (xMove != 0 || zMove != 0) {
      idleAnimation.IsVisible = false;
      runAnimation.IsVisible = true;
      runAnimation.StartFromFirstFrame();
    } else {
      idleAnimation.IsVisible = true;
      runAnimation.IsVisible = false;
      idleAnimation.StartFromFirstFrame();
    }
    base.Move(xMove, zMove);
  }
}
