using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
#pragma warning disable 0649
  [SerializeField] private Transform toFollow;
  [SerializeField] private Player[] players = new Player[4];
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;

  [SerializeField] private bool lockX;
  [SerializeField] private bool lockY;
  [SerializeField] private bool lockZ;
#pragma warning restore 0649

  public Transform target;
  public float smoothTime = 8F;
  private Vector3 velocity = Vector3.zero;

  private Vector3 center = new Vector3(0, 0, 0);

  void Update()
  {
    // Define a target position above and behind the target transform
    //center = new Vector3(0, 0, 0);
    Vector3 oldPosition = transform.position;
    /*
      for (int i = 0; i < 4; i++) {
        center += players[i].transform.position;
      }
      Vector3 targetPosition = center / 4; */
    Vector3 targetPosition = toFollow.position + localOffset;

    // Smoothly move the camera towards that target position
    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

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
