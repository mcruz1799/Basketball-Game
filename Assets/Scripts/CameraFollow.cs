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

  [SerializeField] private GameObject[] leftarrows = new GameObject[4];
  [SerializeField] private GameObject[] rightarrows = new GameObject[4];

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

    //Check if players are offscreen, and if so, make sure the arrows are active.
    for (int i = 0; i < 4; i++) {
      Player p = players[i];
      Vector3 screenPoint = Camera.main.WorldToViewportPoint(p.transform.position);
      bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
      GameObject leftarrow = leftarrows[i];
      GameObject rightarrow = rightarrows[i];
      if (!onScreen) {
        Debug.Log("Player " + i + " is offscreen.");
          if (screenPoint.x < 0) leftarrow.SetActive(true);
          else rightarrow.SetActive(true);
      } else { //They are on screen, make sure the arrows are inactive.
          leftarrow.SetActive(false);
          rightarrow.SetActive(false);
      }
    }
  }
}
