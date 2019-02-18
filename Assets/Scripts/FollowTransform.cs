using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Transform toFollow;
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;
#pragma warning restore 0649

  private void Awake() {
    if (useInspectorOffset) {
      localOffset = transform.position - toFollow.position;
    }
  }

  private void Update() {
    transform.position = toFollow.position + localOffset;
  }
}
