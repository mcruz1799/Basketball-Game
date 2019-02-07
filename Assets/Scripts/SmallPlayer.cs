using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class SmallPlayer : MonoBehaviour, IBallUser, IXzController {
  //Not necessary --yet--.  If we ever add animations for this stuff, will probably need it then.
  //private enum State { Throwing, Jumping, Moving, Falling }

#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox;
  [SerializeField] private float throwDistance;
#pragma warning restore 0649

  private BallUserComponent ballUserComponent;
  private IXzController xzController;

  public SmallPlayer Above { get; private set; }
  public SmallPlayer Below { get; private set; }

  private void Awake() {
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
    Below.Above = null;

    //Set Below to null and notify self + anyone being carried that their height has changed
    Below = null;
    this.OnHeightChanged();

    return true;
  }

  public bool PickUpPlayer() {

    //Can't pick up another SmallPlayer unless your hands are free and you're not being carried
    bool beingCarried = Below != null;
    bool handsFree = !HasBall && Above == null;
    if (beingCarried || !handsFree) {
      return false;
    }

    //Check PickUpPlayer action's hitbox
    RaycastHit[] hits = Physics.BoxCastAll(grabHitbox.center, grabHitbox.bounds.extents, Vector3.zero);
    foreach (RaycastHit h in hits) {
      SmallPlayer other = h.collider.GetComponent<SmallPlayer>();
      if (other.Below == null) {
        other.Below = this;
        Above = other;

        //Can only pick up one person
        break;
      }
    }

    return true;
  }

  public bool ThrowPlayer() {
    if (Above == null) {
      return false;
    }

    return Above.JumpOffPlayer();
  }


  //
  //Miscellaneous helpers
  //

  //Called in response to being grabbed or thrown
  private void OnHeightChanged() {
    Debug.LogWarning("TODO: Change xz-position of grabbed/thrown player(s)");

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
}
