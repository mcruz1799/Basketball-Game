using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
  [SerializeField] private SmallPlayer smallPlayer1;
  //[SerializeField] private SmallPlayer smallPlayer2;
  //[SerializeField] private SmallPlayer smallPlayer3;

  private void Update() {
    float xMove = Input.GetAxis("LeftJoystickX");
    float zMove = -Input.GetAxis("LeftJoystickY");
    smallPlayer1.Move(xMove, zMove);

    float Xrotation = Input.GetAxis("RightJoystickX");
    float Zrotation = Input.GetAxis("RightJoystickY");
    if (!(Xrotation == 0 && Zrotation == 0)) {
      smallPlayer1.SetRotation(Xrotation, Zrotation);
    }
  }
}
