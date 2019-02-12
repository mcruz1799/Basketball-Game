﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class SmallPlayer : MonoBehaviour, IPlayer {
  //Not necessary --yet--.  If we ever add animations for this stuff, will probably need it then.
  //private enum State { Throwing, Jumping, Moving, Falling }
#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox; //Used ONLY in Awake().  Changing this field mid-game does NOTHING.
  [SerializeField] private float jumpDistance;
  [SerializeField] private ScoreComponent.PlayerType _team;
#pragma warning restore 0649

  private BallUserComponent ballUserComponent;
  private IXzController xzController;

  public ScoreComponent.PlayerType Team { get; private set; }
  public TallPlayer Below { get; private set; }

  private void Awake() {
    Team = _team;
    xzController = GetComponent<PlayerMover>();
    ballUserComponent = GetComponent<BallUserComponent>();
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
    Below = null;

    //Assumes ground is at y = 0
    float yGround = 0;
    Vector3 newPosition = transform.position;
    newPosition.y = yGround + transform.lossyScale.y / 2;

    Vector3 throwDirection = new Vector3(xDirection, 0, zDirection).normalized;
    newPosition += throwDirection * jumpDistance;

    //TODO: Check if throw is in bounds?
    transform.SetParent(null, true);
    transform.position = newPosition;
    return true;
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
  //IXzController
  //

  public float X => xzController.X;
  public float Z => xzController.Z;

  public float XLook => xzController.XLook;
  public float ZLook => xzController.ZLook;

  public void Move(float xMove, float zMove) {
    if (Below != null) return;
    xzController.Move(xMove, zMove);
  }

  public void SetRotation(float xLook, float zLook) {
    xzController.SetRotation(xLook, zLook);
  }

  /*
   Possibilities of Pressing A:
   Tip-Off: Gain control of the ball.
   Pass: If Player has the ball, pass it.
  */
  public void PressA(XboxController controller) {
    Debug.Log(HasBall);
    if (HasBall) {
      Pass();
    } else {
      GameManager.S.CheckTipOff(controller);
      JumpOffPlayer();
    }
  }
  public void PressB(XboxController controller) {
    Steal();
  }
}
