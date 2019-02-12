using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class SmallPlayer : Player {
  //Not necessary --yet--.  If we ever add animations for this stuff, will probably need it then.
  //private enum State { Throwing, Jumping, Moving, Falling }
#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox; //Used ONLY in Awake().  Changing this field mid-game does NOTHING.
  [SerializeField] private float jumpDistance;
  [SerializeField] private ScoreComponent.PlayerType _team;
#pragma warning restore 0649

  private BallUserComponent ballUserComponent;
  private IXzController xzController;

  public override bool CanReceivePass => !HasBall;
  public override ScoreComponent.PlayerType Team { get; protected set; }

  public TallPlayer Below { get; private set; }

  private void Awake() {
    Team = _team;
    xzController = GetComponent<PlayerMover>();
    ballUserComponent = GetComponent<BallUserComponent>();

    if (grabHitbox == null) {
      Debug.LogError("SmallPlayer.grabHitbox is null.  HMMMM  <_<");
    }
  }


  //
  //Actions to be associated with an input in InputManager
  //

  public bool JumpOffPlayer() {

    //Can't jump off if you're not being carried
    if (Below == null || !OnThrown(XLook, ZLook)) {
      return false;
    }

    //Below is no longer carrying me
    Below.OnAboveJumpingOff();
    Below = null;
    return true;
  }


  //
  //For use by TallPlayer
  //

  public void OnPickedUp(TallPlayer player) {
    Below = player;
    transform.SetParent(Below.transform, true);

    Vector3 newPosition = Vector3.zero;

    newPosition.y += (this.transform.lossyScale.y + Below.transform.lossyScale.y) / 2;
    transform.localPosition = newPosition;
  }

  public bool OnThrown(float xDirection, float zDirection) {
    //Can't be thrown if there's nobody holding you
    if (Below != null) {
      return false;
    }
    Below = null;

    Vector3 throwOrigin = transform.position;
    throwOrigin.y = 0;

    Vector3 throwDestination;


    //
    //Being thrown into the basket
    //

    if (HasBall) {
      GameObject basket = GameManager.BasketFromTeam(Team);
      //Cast towards basket.  If one of the hits is the basket, and the distance is sufficiently small, auto-target the basket.
      //Check for opposing Team stack, which may be blocking the shot
    }


    //
    //Being thrown into another Player
    //


    //
    //Being thrown onto the ground
    //

    //Assumes ground is at y = 0
    float yGround = 0;
    Vector3 newPosition = transform.position;
    newPosition.y = yGround + transform.lossyScale.y / 2;

    Vector3 throwDirection = new Vector3(xDirection, 0, zDirection).normalized;
    newPosition += throwDirection * jumpDistance;

    //Cast a ray from throwOrigin to newPosition.
    //Find nearest hit that's a Player.
    //Call OnHitByPlayer() (needs to be added to Player class as abstract method)
    //Change newPosition xz-coords to reflect the hit location

    //What about throwing into the basket?
    //If within range of basket, then score

    //TODO: Check if throw is in bounds?
    transform.SetParent(null, true);
    transform.position = newPosition;
    return true;
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
    if (Below != null) return;
    xzController.Move(xMove, zMove);
  }

  public override void SetRotation(float xLook, float zLook) {
    xzController.SetRotation(xLook, zLook);
  }

  /*
   Possibilities of Pressing A:
   Tip-Off: Gain control of the ball.
   Pass: If Player has the ball, pass it.
  */
  public override void PressA(XboxController controller) {
    Debug.Log(HasBall);
    if (HasBall) {
      Pass();
    } else {
      GameManager.S.CheckTipOff(controller);
      JumpOffPlayer();
    }
  }
  public override void PressB(XboxController controller) {
    Steal();
  }
}
