using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
  [SerializeField] private Transform toFollow;
  [SerializeField] private Player[] players = new Player[4];
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;

  public Transform target;
  public float smoothTime = 0.3F;
  private Vector3 velocity = Vector3.zero;

  private Vector3 center = new Vector3(0, 0, 0);

  void Update()
  {
    // Define a target position above and behind the target transform
    center = new Vector3(0, 0, 0);
    for (int i = 0; i < 4; i++) {
      center += players[i].transform.position;
    }
    Vector3 targetPosition = center / 4;

    // Smoothly move the camera towards that target position
    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
  }
}
