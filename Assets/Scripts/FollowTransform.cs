using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Transform toFollow;
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;

  [SerializeField] private bool smoothMotion;
  [Range(1f, 100f)] [SerializeField] private float smoothMotionSpeed;

  [SerializeField] private bool lockX;
  [SerializeField] private bool lockY;
  [SerializeField] private bool lockZ;
#pragma warning restore 0649

  private void Awake() {
    if (!useInspectorOffset) {
      localOffset = transform.position - toFollow.position;
    }
  }

  private void Update() {
    Vector3 oldPosition = transform.position;
    Vector3 newPosition = toFollow.position + localOffset;
    if (lockX) {
      newPosition.x = oldPosition.x;
    }
    if (lockY) {
      newPosition.y = oldPosition.y;
    }
    if (lockZ) {
      newPosition.z = oldPosition.z;
    }

    if (smoothMotion) {
      Vector3 selfToTarget = transform.position - newPosition;
      if (selfToTarget.sqrMagnitude > 0.0025)
      transform.Translate(selfToTarget.normalized * Time.deltaTime * smoothMotionSpeed);
    } else {
      transform.position = newPosition;
    }
  }
}
