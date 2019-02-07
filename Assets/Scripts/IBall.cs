using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBall {
  float Radius { get; }
  void SetPosition(Vector3 newPosition);
  void SetParent(Transform newParent);
}