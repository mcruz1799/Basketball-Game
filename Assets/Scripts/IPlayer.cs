using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer : IBallUser, IXzController {
  ScoreComponent.PlayerType Team { get; }
}