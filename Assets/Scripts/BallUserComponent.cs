using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMover))]
public class BallUserComponent : MonoBehaviour {
#pragma warning disable 0649
  [Range(0f, 10f)] [SerializeField] private float stealCooldown;
  [SerializeField] private float localHeightToHoldBallAt;
  [SerializeField] private float maxPassDistance;
  [SerializeField] private AudioClip successfulSteal;
  [SerializeField] private AudioClip successfulPass;
#pragma warning restore 0649

  private IXzController xzController; //Needed only for XLook and ZLook
  private IBall heldBall;

  private float stealCooldownRemaining;
  public bool CanSteal => stealCooldownRemaining <= 0f;

  private void Awake() {
    xzController = GetComponent<PlayerMover>();

    if (maxPassDistance == 0) {
      Debug.LogWarning("BallUserComponent.maxPassDistance equals 0.  HMMMM  <_<");
    }

    StartCoroutine(StealCooldownRoutine());
  }

  public bool HasBall { get { return heldBall != null; } }

  public void HoldBall(IBall ball) {
    heldBall = ball;
    if (ball != null) {
      ball.SetParent(transform);

      Vector3 ballPosition = Vector3.zero;
      ballPosition.y = localHeightToHoldBallAt;
      ballPosition.z = transform.lossyScale.z / 2 + ball.Radius;

      ball.SetPosition(ballPosition);
    }
  }

  public bool Steal(BoxCollider grabHitbox) {
    if (HasBall || !CanSteal) {
      Debug.LogFormat("Cannot steal.  HasBall: {0}   stealCooldownRemaining: {1}", HasBall, stealCooldownRemaining);
      return false;
    }

    Collider[] hits = Physics.OverlapBox(grabHitbox.transform.TransformPoint(grabHitbox.center), grabHitbox.bounds.extents);
    foreach (Collider h in hits) {
      BallUserComponent other = h.GetComponent<BallUserComponent>();
      if (other != null && other.heldBall != null) {
        stealCooldownRemaining = stealCooldown;
        HoldBall(other.heldBall);
        other.heldBall = null;
        SoundManager.Instance.Play(successfulSteal);
        return true;
      }
    }

    Debug.Log("Nobody to Steal from detected");
    return false;
  }

  public void Pass() {
    float yGround = 0 + 0.15f;

    Vector3 passOrigin = transform.position;
    passOrigin.y = yGround;
    Vector3 passDirection = new Vector3(xzController.XLook, 0, xzController.ZLook);

    RaycastHit[] hits = Physics.RaycastAll(passOrigin, passDirection, maxPassDistance);
    float nearestPlayerHitDistance = Mathf.Infinity;
    Player nearestPlayerHit = null;
    foreach (RaycastHit hit in hits) {
      Player player = hit.collider.GetComponent<Player>();
      if (player != null && hit.distance < nearestPlayerHitDistance) {
        nearestPlayerHitDistance = hit.distance;
        nearestPlayerHit = player;
      }
    }

    if (nearestPlayerHit != null && nearestPlayerHit.CanReceivePass) {
      nearestPlayerHit.HoldBall(heldBall);
      Debug.Log("Pass Recipient:" + nearestPlayerHit.transform.parent.name);
      heldBall = null;
      SoundManager.Instance.Play(successfulPass);
    } else {
      Debug.Log("No Pass Recipient.");
    }
  }

  private IEnumerator StealCooldownRoutine() {
    while (true) {
      if (stealCooldownRemaining <= 0f) {
        yield return new WaitUntil(() => stealCooldownRemaining > 0f);
      }

      stealCooldownRemaining -= 0.1f;
      yield return new WaitForSeconds(0.1f);
    }
  }
}
