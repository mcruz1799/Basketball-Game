﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBallUser {
  bool HasBall { get; }

  void Steal();
  void Pass();
}