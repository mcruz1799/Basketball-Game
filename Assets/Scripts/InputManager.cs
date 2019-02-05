using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
  SmallPlayer smallPlayer1;
  //SmallPlayer smallPlayer2;
  //SmallPlayer smallPlayer3;

  private void Update() {
    float xMove = Input.GetAxis("LeftJoystickX");
    float zMove = -Input.GetAxis("LeftJoystickY");
    smallPlayer1.Move(xMove, zMove);

    float Xrotation = Input.GetAxis("RightJoystickX");
    float Zrotation = Input.GetAxis("RightJoystickY");
    smallPlayer1.SetRotation(Xrotation, Zrotation);
  }
}
