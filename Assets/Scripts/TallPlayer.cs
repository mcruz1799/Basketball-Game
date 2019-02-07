﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class TallPlayer : MonoBehaviour, IPlayer {
#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox;
  [SerializeField] private float throwDistance;
  [SerializeField] private ScoreComponent.PlayerType _team;

#pragma warning restore 0649

  private IXzController xzController;
  private BallUserComponent ballUserComponent;

  public ScoreComponent.PlayerType Team { get; private set; }
  public SmallPlayer Above { get; private set; }

  private void Awake() {
    Team = _team;
    xzController = GetComponent<PlayerMover>();
    ballUserComponent = GetComponent<BallUserComponent>();
  }

  public void ThrowSmallPlayer() {
    Above.OnThrown();
    Above = null;
  }

  public bool PickUpSmallPlayer() {

    //Can't pick up SmallPlayer unless your hands are free and you're not being carried
    bool handsFree = !HasBall;
    if (!handsFree) {
      return false;
    }

    //Check grab hitbox
    RaycastHit[] hits = Physics.BoxCastAll(grabHitbox.center, grabHitbox.bounds.extents, Vector3.zero);
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
    Debug.LogWarning("TallPlayer.OnAboveJumpingOff not yet implemented");
    //Anything else to actually do...?
  }

  //IBallUser
  //-----------------------------------------------
  public void Pass() {
    ballUserComponent.Pass(xzController.XLook, xzController.ZLook);
  }

  public bool Steal() {
    return ballUserComponent.Steal(grabHitbox);
  }
  public bool HasBall => ballUserComponent.HasBall;


  //IXzController
  //------------------------------------------
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
   Pick-Up Small Player: If Player is in range, can pickup the small player.
                                                   */
  public void PressA(XboxController controller)
  {
        if (HasBall)
        {
            Pass();
        } else
        {
            //TODO: Check Tip-Off, or attempt to pick-up small player.
        }
  }
}
