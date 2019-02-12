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

  public override bool CanMove => base.CanMove && Below == null;
  public TallPlayer Below { get; private set; }


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

    RaycastHit[] hits;
    float yGround = 0f + 0.05f; //Cast rays slightly above ground level
    Vector3 throwOrigin = transform.position; throwOrigin.y = yGround;

    //TODO: Allow these to be customized
    float scoreDistance = 1f;
    float throwDistance = 1f;

    //Throw in the direction specified by xDirection and zDirection
    Vector3 throwDirection = new Vector3(xDirection, 0, zDirection).normalized;
    Vector3 throwDestination = transform.position + throwDirection * throwDistance;

    //HOWEVER, auto-target the basket if
    //  SmallPlayer has the ball AND
    //  The basket is close enough
    if (HasBall) {
      GameObject basket = GameManager.BasketFromTeam(Team);
      Vector3 from = new Vector3(throwOrigin.x, basket.transform.position.y, throwOrigin.z);
      Vector3 direction = basket.transform.position - from;
      hits = Physics.RaycastAll(from, direction, scoreDistance);
      foreach (RaycastHit hit in hits) {
        if (hit.collider.gameObject.Equals(basket)) {
          throwDestination = hit.point;
          break;
        }
      }
    }

    //Now we check whether we hit another player on the way to the destination.
    //For raycasting purposes, we set the y to ground level.
    throwDestination.y = yGround;


    //
    //Being thrown into another Player
    //

    Vector3 originToDestination = throwDestination - throwOrigin;
    hits = Physics.RaycastAll(throwOrigin, originToDestination, originToDestination.magnitude);
    RaycastHit nearestPlayerHit = default;
    Player nearestPlayer = null;
    foreach (RaycastHit hit in hits) {
      Player player = hit.collider.GetComponent<Player>();
      if (player != null && (nearestPlayer == null || hit.distance < nearestPlayerHit.distance)) {
        nearestPlayerHit = hit;
        nearestPlayer = player;
      }
    }

    if (nearestPlayer != null) {
      nearestPlayer.Stun();
      throwDestination = nearestPlayerHit.point;
    }

    //TODO: Check if throw is in bounds?
    throwDestination.y = yGround + transform.lossyScale.y / 2;
    transform.SetParent(null, true);
    transform.position = throwDestination;
    return true;
  }


  //
  //IXzController
  //

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
