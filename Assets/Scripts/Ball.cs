using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall {
#pragma warning disable 0649
  [Range(1f, 100f)] [SerializeField] private float moveAnimationSpeed = 1f;
#pragma warning restore 0649

  private Vector3? targetLocalPosition;

  public float Radius { get { return transform.lossyScale.x / 2; } }

  private void Awake() {
    StartCoroutine(GfxRoutine());
  }

  //For use only in BallUserComponent.HoldBall
  public void SetPosition(Vector3 newLocalPosition) {
    if (transform.parent != null) {
      newLocalPosition.x /= transform.parent.lossyScale.x;
      newLocalPosition.y /= transform.parent.lossyScale.y;
      newLocalPosition.z /= transform.parent.lossyScale.z;
      targetLocalPosition = newLocalPosition;
    } else {
      targetLocalPosition = newLocalPosition;
    }
  }

  //For use only in BallUserComponent.HoldBall
  public void SetParent(Transform newParent) {
    transform.SetParent(newParent, true);
  }

  private IEnumerator GfxRoutine() {
    while (true) {
      if (targetLocalPosition == null) {
        yield return new WaitUntil(() => targetLocalPosition.HasValue);
      }

      Vector3 selfToTarget = (targetLocalPosition.Value - transform.localPosition);
      if (selfToTarget.sqrMagnitude < 0.04f) {
        transform.localPosition = targetLocalPosition.Value;
        targetLocalPosition = null;
      } else {
        transform.localPosition += moveAnimationSpeed * Time.deltaTime * selfToTarget.normalized;
      }
      yield return null;
    }
  }
}