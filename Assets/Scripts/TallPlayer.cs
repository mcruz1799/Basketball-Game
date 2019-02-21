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
  public override bool CanReceivePass => base.CanReceivePass && Above == null;

  private bool idleIsFlashing;

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
    if (Above == null || !Above.OnThrown(XLook, ZLook)) {
      return false;
    }

    Above = null;
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
    Collider[] hits = Physics.OverlapBox(grabHitbox.transform.TransformPoint(grabHitbox.center), grabHitbox.bounds.extents);
    foreach (Collider h in hits) {
      SmallPlayer other = h.GetComponent<SmallPlayer>();
      if (other != null && other.Team == Team && other.Below == null) {
        other.OnPickedUp(this);
        Above = other;
        Debug.LogFormat("{0} picked up {1}", transform.parent.name, other.transform.parent.name);
        return true;
      }
    }

    Debug.Log("Nobody to pick up in grabHitbox");
    return false;
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

  private IEnumerator FlashSprite() {
    while (true) {
      idleAnimation.IsVisible = false;
      yield return new WaitForSeconds(.5f);
      idleAnimation.IsVisible = true;
      yield return new WaitForSeconds(.5f);
    }
  }

  protected override IEnumerator StunRoutine() {
    idleIsFlashing = true;
    Coroutine flashRoutine = StartCoroutine(FlashSprite()); //Want this running concurrently with StunRoutine
    yield return base.StunRoutine();
    StopCoroutine(flashRoutine);
    idleIsFlashing = false;
  }


  //
  //IXzController
  //

  /*
   Possibilities of Pressing A:
   Tip-Off: Gain control of the ball.
   Pass: If Player has the ball, pass it.
   Pick-Up Small Player: If Player is in range, can pickup the small player.
  */
  public override void AButtonDown(XboxController controller) {
    if (HasBall) {
      //Pass();
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
    if (CanMove && (xMove != 0 || zMove != 0)) {
      idleAnimation.IsVisible = false;
      runAnimation.FlipX = zMove > 0;
      idleAnimation.FlipX = runAnimation.FlipX;
      if (!runAnimation.IsVisible) {
        runAnimation.IsVisible = true;
        runAnimation.StartFromFirstFrame();
      }
    } else {
      runAnimation.IsVisible = false;
      if (!idleAnimation.IsVisible && !idleIsFlashing) {
        idleAnimation.IsVisible = true;
        idleAnimation.StartFromFirstFrame();
      }
    }
    base.Move(xMove, zMove);
  }
}
