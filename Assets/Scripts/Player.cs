using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

//This is a relic from when there was no abstract Player class.
//  Needed the abstract class in order to use GetComponent<Player>() in BallUser.Pass()
//It is preserved so as not to break existing code.
public interface IPlayer : IBallUser, IXzController {
  ScoreComponent.PlayerType Team { get; }
  void PressA(XboxController controller);
  void PressB(XboxController controller);
}

public abstract class Player : MonoBehaviour, IPlayer {

  //Specific to Player
  public abstract ScoreComponent.PlayerType Team { get; protected set; }
  public abstract void PressA(XboxController controller);
  public abstract void PressB(XboxController controller);

  //IXzController
  public abstract float X { get; }
  public abstract float Z { get; }
  public abstract float XLook { get; }
  public abstract float ZLook { get; }
  public abstract void Move(float xMove, float zMove);
  public abstract void SetRotation(float xLook, float zLook);

  //IBallUser
  public abstract bool HasBall { get; }
  public abstract void HoldBall(IBall ball);
  public abstract void Pass();
  public abstract bool Steal();
}
