using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class TallPlayer : MonoBehaviour, IPlayer {
#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox;
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

  public bool ThrowSmallPlayer() {
    if (Above == null || !Above.OnThrown(XLook, ZLook)) {
      return false;
    }

    Above = null;
    return true;
  }

  public bool PickUpSmallPlayer() {
    Debug.Log("Picking up.");
    //Can't pick up SmallPlayer unless your hands are free and you're not being carried
    bool handsFree = !HasBall && Above == null;
    if (!handsFree) {
      return false;
    }

    //Check grab hitbox
    Vector3 distance = grabHitbox.transform.position - transform.position;
    RaycastHit[] hits = Physics.BoxCastAll(transform.position, grabHitbox.bounds.extents,distance);
    Debug.Log("Hits: " + hits.Length);
    foreach (RaycastHit h in hits) {
      SmallPlayer other = h.collider.GetComponent<SmallPlayer>();
      Debug.Log("Other:" + other);
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
            GameManager.S.CheckTipOff(controller);
            PickUpSmallPlayer();
        }
  }
}
