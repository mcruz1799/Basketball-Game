﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IXzController {
  float X { get; }
  float Z { get; }

  float xLook { get; }
  float zLook { get; }

  void Move();
  void setRotation(Vector3 rotation);
}
