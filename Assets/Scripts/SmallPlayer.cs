using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class SmallPlayer : MonoBehaviour, IBallUser {
  //Not necessary --yet--.  If we ever add animations for this stuff, will probably need it then.
  //private enum State { Transitioning }

#pragma warning disable 0649
  [SerializeField] private BoxCollider grabHitbox;
#pragma warning restore 0649

  private IBallUser ballUserComponent;
  private IXzController xzController;

  public SmallPlayer Above { get; private set; }
  public SmallPlayer Below { get; private set; }

  //Returns the number of SmallPlayers below this
  public int NumberBelow {
    get {
      int result = 0;
      foreach (SmallPlayer p in GetBelow()) {
        result += 1;
      }
      return result;
    }
  }

  //Returns the number of SmallPlayers above this
  public int NumberAbove {
    get {
      int result = 0;
      foreach (SmallPlayer p in GetAbove()) {
        result += 1;
      }
      return result;
    }
  }

  public int TotemHeight {
    get {
      return NumberBelow + 1 + NumberAbove;
    }
  }

  private void Awake() {
    ballUserComponent = GetComponent<BallUserComponent>();
    //xzController = GetComponent<PlayerMover>();
  }

  private bool JumpOffPlayer() {

    //Can't jump off if you're not being carried
    if (Below == null) {
      return false;
    }

    //Below is no longer carrying me
    Below.Above = null;

    //Set Below to null and notify self + anyone being carried that their height has changed
    Below = null;
    this.OnHeightChanged();
    foreach (SmallPlayer p in GetAbove()) {
      p.OnHeightChanged();
    }

    return true;
  }

  private bool PickUpPlayer() {

    //Can't pick up a another SmallPlayer unless your hands are free and you're not being carried
    if (Above != null || Below != null || HasBall) {
      return false;
    }

    //Check PickUpPlayer action's hitbox
    RaycastHit[] hits = Physics.BoxCastAll(grabHitbox.center, grabHitbox.bounds.extents, Vector3.zero);
    foreach (RaycastHit h in hits) {
      SmallPlayer other = h.collider.GetComponent<SmallPlayer>();
      if (other.Below == null) {
        other.Below = this;
        Above = other;

        foreach (SmallPlayer p in GetAbove()) {
          p.OnHeightChanged();
        }

        //Can only pick up one person
        break;
      }
    }

    return true;
  }

  private bool ThrowPlayer() {
    if (Above == null) {
      return false;
    }

    return Above.JumpOffPlayer();
  }

  private void OnHeightChanged() {
    Vector3 newPos = transform.position;
    newPos.y = NumberBelow * transform.lossyScale.y + (transform.lossyScale.y / 2);

    transform.position = newPos;
  }

  private IEnumerable<SmallPlayer> GetAbove() {
    SmallPlayer temp = this;
    while (temp.Above != null) {
      yield return temp.Above;
      temp = temp.Above;
    }
  }

  private IEnumerable<SmallPlayer> GetBelow() {
    SmallPlayer temp = this;
    while (temp.Below != null) {
      yield return temp.Below;
      temp = temp.Below;
    }
  }


  //
  //IBallUser
  //

  public bool HasBall => ballUserComponent.HasBall;

  public void Pass() {
    ballUserComponent.Pass();
  }

  public void Steal() {
    ballUserComponent.Steal();
  }
}
