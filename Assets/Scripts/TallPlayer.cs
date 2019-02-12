using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class TallPlayer : Player {
#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox; //Used ONLY in Awake().  Changing this field mid-game does NOTHING.
  [SerializeField] private ScoreComponent.PlayerType _team;
#pragma warning restore 0649

  private IXzController xzController;
  private BallUserComponent ballUserComponent;

  public override ScoreComponent.PlayerType Team { get; protected set; }
  public override bool CanReceivePass => Above == null && !HasBall;

  public SmallPlayer Above { get; private set; }

  private void Awake() {
    Team = _team;
    xzController = GetComponent<PlayerMover>();
    ballUserComponent = GetComponent<BallUserComponent>();

    if (grabHitbox == null) {
      Debug.LogError("TallPlayer.grabHitbox is null.  HMMMM  <_<");
    }
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
      return false;
    }

    //Check grab hitbox
    Vector3 selfToHitbox = grabHitbox.transform.position - transform.position;
    RaycastHit[] hits = Physics.BoxCastAll(transform.position, grabHitbox.bounds.extents, selfToHitbox, Quaternion.identity, selfToHitbox.magnitude);
    foreach (RaycastHit h in hits) {
      SmallPlayer other = h.collider.GetComponent<SmallPlayer>();
      if (other != null && other.Team == Team && other.Below == null) {
        other.OnPickedUp(this);
        break;
      }
    }

    return true;
  }

  public void OnAboveJumpingOff() {
    Above = null;
  }

  //
  //IBallUser
  //

  public override bool HasBall => ballUserComponent.HasBall;

  public override void Pass() {
    ballUserComponent.Pass();
  }

  public override bool Steal() {
    return ballUserComponent.Steal(grabHitbox);
  }

  public override void HoldBall(IBall ball) {
    ballUserComponent.HoldBall(ball);
  }


  //
  //IXzController
  //

  public override float X => xzController.X;
  public override float Z => xzController.Z;

  public override float XLook => xzController.XLook;
  public override float ZLook => xzController.ZLook;

  public override void Move(float xMove, float zMove) {
    xzController.Move(xMove, zMove);
  }
  public override void SetRotation(float xLook, float zLook) {
    xzController.SetRotation(xLook, zLook);
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
  }
}
