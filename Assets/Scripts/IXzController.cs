using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public interface IXzController {
  float Speed { get; set; }

  float X { get; }
  float Z { get; }

  float XLook { get; }
  float ZLook { get; }

  void Move(float xDirection, float zDirection);
  void SetRotation(float xLook, float zLook);
}
