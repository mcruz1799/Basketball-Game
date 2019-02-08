using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class SmallPlayer : MonoBehaviour, IPlayer {
  //Not necessary --yet--.  If we ever add animations for this stuff, will probably need it then.
  //private enum State { Throwing, Jumping, Moving, Falling }
#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox;
  [SerializeField] private float jumpDistance;
  [SerializeField] private ScoreComponent.PlayerType _team;
#pragma warning restore 0649

  private BallUserComponent ballUserComponent;
  private IXzController xzController;

  public ScoreComponent.PlayerType Team { get; private set; }
  public TallPlayer Below { get; private set; }


  private void Awake() {
    Team = _team;
    ballUserComponent = GetComponent<BallUserComponent>();
    xzController = GetComponent<PlayerMover>();
  }


  //
  //Actions to be associated with an input in InputManager
  //

  public bool JumpOffPlayer() {

    //Can't jump off if you're not being carried
    if (Below == null) {
      return false;
    }

    //Below is no longer carrying me
    Below.OnAboveJumpingOff();
    Below = null;

    Debug.LogWarning("SmallPlayer.JumpOffPlayer is not yet implemented");
    //Change height, change xz-position

    return true;
  }

  public void OnPickedUp(TallPlayer player) {
    Below = player;
    Debug.LogWarning("SmallPlayer.OnPickedUp is not yet implemented");
    //Change height, change xz-position
  }

  public void OnThrown() {
    Below = null;
    Debug.LogWarning("SmallPlayer.OnThrown is not yet implemented");
    //Change height, change xz-position
  }


  //
  //Miscellaneous helpers
  //

  //Called in response to being grabbed or thrown
  private void OnHeightChanged() {
    Debug.LogWarning("TODO: Change xz-position of SmallPlayer");

    Vector3 newPos = transform.position;

    //Assumes ground is at y = 0 and adjusts for height of self.
    //Alternative: enable rigidbody y-constraint iff Below == null?
    newPos.y = transform.lossyScale.y / 2; 

    //newPos += new Vector3(XLook, 0, ZLook) * throwDistance;

    transform.position = newPos;
  }


  //
  //IBallUser
  //

  public bool HasBall => ballUserComponent.HasBall;

  public void Pass() {
    ballUserComponent.Pass(xzController.XLook, xzController.ZLook);
  }

  public bool Steal() {
    return ballUserComponent.Steal(grabHitbox);
  }


  //
  //IXzController
  //

  public float X => xzController.X;
  public float Z => xzController.Z;

  public float XLook => xzController.XLook;
  public float ZLook => xzController.ZLook;

  public void Move(float xMove, float zMove) {
    xzController.Move(xMove, zMove);
  }

  public void SetRotation(float xLook, float zLook) {
    xzController.SetRotation(xLook, zLook);
  }
  
  /*Possibilities of Pressing A:
   Tip-Off: Gain control of the ball.
   Pass: If Player has the ball, pass it.
                                                   */
  public void PressA(XboxController controller)
  {
        if (HasBall)
        {
            Pass();
        } else
        {
            //TODO: Check for Tip-Off.
            GameManager.S.CheckTipOff(controller);
        }
  }
}
