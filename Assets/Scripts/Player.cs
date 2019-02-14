using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public interface IPlayer : IBallUser, IXzController {
  ScoreComponent.PlayerType Team { get; }
  void PressA(XboxController controller);
  void PressB(XboxController controller);
}

//Exists only so GetComponent can be used.  When defining variables/fields/properties, please use IPlayer instead.
public abstract class Player : MonoBehaviour, IPlayer {
#pragma warning disable 0649
    [SerializeField] protected BoxCollider grabHitbox;
    [SerializeField] private ScoreComponent.PlayerType _team;
#pragma warning restore 0649

    private IXzController xzController;
    private BallUserComponent ballUserComponent;

    public ScoreComponent.PlayerType Team { get; private set; }

    private float dashTimer = 3;
    public bool dashRefillPenalty { get; private set; } = true;

    protected enum TPState { Default, HoldingSmall } //Different Player states that effect movement.
    public enum DashState { Default, Dash } //State for Dash
    protected Dictionary<TPState, float> StateSpeed = new Dictionary<TPState, float>() //Speed multipler related to particular state.
  {
      {TPState.Default, 1.0f },
      {TPState.HoldingSmall, 0.25f },
  };
    protected Dictionary<DashState, float> DashSpeed = new Dictionary<DashState, float>() //Speed multipler related to particular state.
  {
      {DashState.Default, 1.0f },
      {DashState.Dash, 2.0f },
  };



    protected TPState currentTPState = TPState.Default;
    public DashState currentDashState = DashState.Default;

  //Specific to Player
  public virtual bool CanRotate => !IsStunned;
  public virtual bool CanMove => !IsStunned;
  public abstract void PressA(XboxController controller);
  public abstract void PressB(XboxController controller);

  protected virtual void Awake() {
    Team = _team;
    xzController = GetComponent<PlayerMover>();
    ballUserComponent = GetComponent<BallUserComponent>();

    if (grabHitbox == null) {
      Debug.LogError("SmallPlayer.grabHitbox is null.  HMMMM  <_<");
    }
  }

  //Stun functionality
  private bool IsStunned { get; set; }
  private IEnumerator StunRoutine() {
    yield return new WaitForSeconds(2f);
    IsStunned = false;
  }
  public virtual void Stun() {
    if (!IsStunned) {
      IsStunned = true;
    }
  }


  //
  //IXzController
  //

  public float X => xzController.X;
  public float Z => xzController.Z;

  public float XLook => xzController.XLook;
  public float ZLook => xzController.ZLook;

  public Vector3 Move(float xMove, float zMove) {
    if (CanMove) {
      float tpspeed = StateSpeed[currentTPState];
      float dashspeed = DashSpeed[currentDashState];
      Vector3 result = xzController.Move(tpspeed * dashspeed * xMove, tpspeed * dashspeed * zMove);
    }
    return new Vector3(X, 0, Z);
  }
  public void SetRotation(float xLook, float zLook) {
    if (CanRotate) {
      xzController.SetRotation(xLook, zLook);
    }
  }


  //
  //IBallUser
  //

  public bool HasBall => ballUserComponent.HasBall;

  public void Pass() {
    ballUserComponent.Pass();
  }

  public bool Steal() {
    return ballUserComponent.Steal(grabHitbox);
  }

  public void HoldBall(IBall ball) {
    ballUserComponent.HoldBall(ball);
  }

    //
    // Dash Functionality
    //
  private IEnumerator DashActive()
  {
        while (dashTimer > 0)
        {
            dashTimer -= .25f;
            yield return new WaitForSeconds(.25f);
            //TODO: Add Set Stamina Bar here.
        } //Completely depleted timer, penalty accrued.
        dashRefillPenalty = true;
  }
  private IEnumerator DashInactive()
  {
        while (dashTimer < 3)
        {
            dashTimer += .25f;
            yield return new WaitForSeconds(.25f);
        }
        dashRefillPenalty = false;
  }
}
