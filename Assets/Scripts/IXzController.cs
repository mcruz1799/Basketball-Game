using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IXzController {
  float X { get; }
  float Z { get; }

  float XLook { get; }
  float ZLook { get; }

  void Move(float xMove, float zMove);
  void SetRotation(float xLook, float zLook);
  void PressA();
}
