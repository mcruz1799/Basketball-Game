using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall {
#pragma warning disable 0649
  [Range(0.1f, 10f)] [SerializeField] private float moveAnimationSpeed;
#pragma warning restore 0649

  private Vector3? targetLocalPosition;

  public float Radius { get { return transform.lossyScale.x; } }

  private void Awake() {
    StartCoroutine(GfxRoutine());
  }

  //For use only in BallUserComponent.HoldBall
  public void SetPosition(Vector3 newLocalPosition) {
    targetLocalPosition = newLocalPosition;
    StartCoroutine(GfxRoutine());
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
      } else {
        transform.localPosition += moveAnimationSpeed * Time.deltaTime * selfToTarget.normalized;
      }
      yield return null;
    }
  }
}