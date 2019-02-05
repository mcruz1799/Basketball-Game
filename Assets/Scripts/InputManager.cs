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

    float xlook = Input.GetAxis("RightJoystickX");
    float zLook = Input.GetAxis("RightJoystickY");
    if (!(xlook == 0 && zLook == 0)) {
      smallPlayer1.SetRotation(xlook, zLook);
    }
  }
}
