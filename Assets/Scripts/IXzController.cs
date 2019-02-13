using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public interface IXzController {
  float X { get; }
  float Z { get; }

  float XLook { get; }
  float ZLook { get; }

  Vector3 Move(float xMove, float zMove);
  void SetRotation(float xLook, float zLook);
}
