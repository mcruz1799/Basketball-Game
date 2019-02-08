using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall {
  public IPlayer Owner { get; private set; }

  public float Radius { get { return transform.lossyScale.x; } }

  public void SetPosition(Vector3 newPosition) {
    transform.localPosition = newPosition;
  }

  public void SetParent(Transform newParent) {
    transform.SetParent(newParent, true);
  }
  public void SetOwner(IPlayer player)
  {
        Owner = player;
  }
}