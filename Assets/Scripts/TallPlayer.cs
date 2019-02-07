using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallPlayer : MonoBehaviour, IXzController
{
  public float X { get; }
  public float Z { get; }

  public float XLook { get; }
  public float ZLook { get; }

  public void Move(float xMove, float zMove){
      return;
  }
  public void SetRotation(float xLook, float zLook){
      return;
  }
}
