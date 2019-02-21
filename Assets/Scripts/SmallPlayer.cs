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
  [SerializeField] private float throwDistance = 1f;

  //DO NOT REFERENCE THESE DIRECTLY.
  //USE THE ISpriteAnimator FIELDS INSTEAD
  [SerializeField] private SpriteAnimator _spIdle;
  [SerializeField] private SpriteAnimator _spRun;
  private Vector3 startingScale;
#pragma warning restore 0649

  public override bool CanMove => base.CanMove && Below == null;
  public TallPlayer Below { get; private set; }

  private ISpriteAnimator idleAnimation;
  private ISpriteAnimator runAnimation;

  private bool idleIsFlashing = false;
  protected override void Awake() {
    base.Awake();

    //Initialize ISpriteAnimators
    idleAnimation = _spIdle;
    runAnimation = _spRun;

    idleAnimation.IsLooping = true;
    runAnimation.IsLooping = true;

    startingScale = transform.localScale;
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
    transform.localScale = startingScale;
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
    newPosition.y /= transform.parent.lossyScale.y;
    transform.localPosition = newPosition;
  }

  public bool OnThrown(float xDirection, float zDirection) {
    //Can't be thrown if there's nobody holding you
    if (Below == null) {
      return false;
    }

    RaycastHit[] hits;
    float skinWidth = 0.15f;
    float yGround = 0f;

    //Cast rays slightly above ground level
    Vector3 raycastOrigin = transform.position; raycastOrigin.y = yGround + skinWidth;

    //Throw in the direction specified by xDirection and zDirection
    Vector3 throwDirection = new Vector3(xDirection, 0, zDirection).normalized;
    Vector3 throwDestination = transform.position + throwDirection * throwDistance;


    //
    //Being thrown into another Player
    //

    //Now we check whether we hit another player on the way to the destination.
    //For raycasting purposes, we set the y to ground level.
    throwDestination.y = yGround;

    Vector3 originToDestination = throwDestination - raycastOrigin; originToDestination.y = 0;
    hits = Physics.RaycastAll(raycastOrigin, originToDestination, originToDestination.magnitude);
    // Debug.Log("Raycast Origin:" + raycastOrigin);
    // Debug.Log("Raycast Desination:" + raycastOrigin + originToDestination);
    Debug.DrawRay(raycastOrigin, originToDestination, Color.cyan);
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
      throwDestination = nearestPlayerHit.point;
    }

    if (GameManager.S.PositionIsOutOfBounds(throwDestination)) {
      Debug.Log("Throw would put SmallPlayer out of bounds");
      return false;
    }

    //Update game state if throw is legal

    if (nearestPlayer != null) {
      Debug.Log("Threw SmallPlayer into another player, stunning them  :D");
      nearestPlayer.Stun();
    }
    Below = null;

    throwDestination.y = yGround + transform.lossyScale.y / 2 + .2f;
    transform.SetParent(OriginalParent, true);
    transform.position = throwDestination;
    transform.localScale = startingScale;
    return true;
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

  protected override void PerformDashAction(){ //STEAL
    Steal();
  }


  //
  //IXzController
  //

  /*
   Possibilities of Pressing A:
   Tip-Off: Gain control of the ball.
   Pass: If Player has the ball, pass it.
  */
  public override void AButtonDown(XboxController controller) {
    if (HasBall) {
      //Pass();
    } else {
      GameManager.S.CheckTipOff(controller);
      JumpOffPlayer();
    }
  }
  public override void RTButtonDown(XboxController controller) {
    StartDashing();
  }
  public override void RTButtonUp(XboxController controller) {
    StopDashing();
  }
  public override void BButtonDown(XboxController controller) {
    Steal();
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
