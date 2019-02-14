using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public interface IPlayer : IBallUser, IXzController {
  ScoreComponent.PlayerType Team { get; }
  void AButtonDown(XboxController controller);
  void BButtonDown(XboxController controller);
  void BButtonUp(XboxController controller);
}

//Exists only so GetComponent can be used.  When defining variables/fields/properties, please use IPlayer instead.
public abstract class Player : MonoBehaviour, IPlayer {
#pragma warning disable 0649
  [SerializeField] protected BoxCollider grabHitbox;
  [SerializeField] private ScoreComponent.PlayerType _team;
#pragma warning restore 0649

  private PlayerMover xzController;
  private BallUserComponent ballUserComponent;

  public ScoreComponent.PlayerType Team { get; private set; }

  //Specific to Player
  public virtual bool CanRotate => !IsStunned;
  public virtual bool CanMove => !IsStunned;
  public abstract void AButtonDown(XboxController controller);
  public abstract void BButtonDown(XboxController controller);
  public abstract void BButtonUp(XboxController controller);

  protected virtual void Awake() {
    Team = _team;
    xzController = GetComponent<PlayerMover>();
    ballUserComponent = GetComponent<BallUserComponent>();
    StartCoroutine(DashRoutine());

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

  public virtual float Speed => xzController.Speed * (IsDashing ? 1f : 2f);

  public float X => xzController.X;
  public float Z => xzController.Z;

  public float XLook => xzController.XLook;
  public float ZLook => xzController.ZLook;

  public void Move(float xMove, float zMove) {
    if (CanMove) {
      float oldSpeed = xzController.Speed;
      xzController.Speed = Speed;
      xzController.Move(xMove, zMove);
      xzController.Speed = oldSpeed;
    }
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

  private float dashTimer = 3;
  public bool dashRefillPenalty { get; private set; } = true;
  public bool IsDashing { get; private set; }

  public void StartDashing() {
    if (!dashRefillPenalty) {
      IsDashing = true;
    }
  }

  public void StopDashing() {
    IsDashing = false;
  }

  private IEnumerator DashRoutine() {
    while (true) {
      yield return new WaitForSeconds(0.25f);

      if (IsDashing) {
        dashTimer -= 0.25f;
        if (dashTimer <= 0f) {
          dashTimer = 0f;
          dashRefillPenalty = true;
          StopDashing();
        }
      } else {
        dashTimer += 0.25f;
        if (dashTimer >= 3f) {
          dashTimer = 3f;
          dashRefillPenalty = false;
        }
      }
    }
  }
}
