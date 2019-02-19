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

    if (smoothMotion) {
      Vector3 selfToTarget = newPosition - oldPosition;
      if (selfToTarget.sqrMagnitude <= 1f) { //distance bw camera and obj
        transform.position = newPosition;
      } else {
        //transform.Translate(selfToTarget.normalized * Time.deltaTime * smoothMotionSpeed);
        //Vector3 velocity = new Vector3(smoothMotionSpeed, 0, smoothMotionSpeed);
        //transform.position = Vector3.SmoothDamp(oldPosition, newPosition, ref velocity, 1f);
        transform.position = Vector3.MoveTowards(oldPosition, newPosition, smoothMotionSpeed*Time.deltaTime);
      }
    } else {
      transform.position = newPosition;
    }
    Vector3 finalPosition = transform.position;
    if (lockX) {
       finalPosition.x = oldPosition.x;
    }
    if (lockY) {
      finalPosition.y = oldPosition.y;
    }
    if (lockZ) {
      finalPosition.z = oldPosition.z;
    }
    transform.position = finalPosition;

  }
}
