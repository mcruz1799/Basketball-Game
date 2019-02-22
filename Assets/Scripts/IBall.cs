using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBall {
  float Radius { get; }

  void SetPosition(Vector3 newLocalPosition, bool animateChange=true);
  void SetParent(Transform newParent);
}