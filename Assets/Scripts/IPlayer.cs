using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public interface IPlayer : IBallUser, IXzController {
  ScoreComponent.PlayerType Team { get; }
  void PressA(XboxController controller);
  void PressB(XboxController controller);
}