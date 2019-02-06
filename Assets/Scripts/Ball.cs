﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall {
  public float Radius { get { return transform.lossyScale.x; } }

  public void SetPosition(Vector3 newPosition) {
    transform.position = newPosition;
  }

  public void SetParent(Transform newParent) {
    transform.SetParent(newParent, true);
  }
}

public interface IBall {
  float Radius { get; }
  void SetPosition(Vector3 newPosition);
  void SetParent(Transform newParent);
}