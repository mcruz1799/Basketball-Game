using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class InputManager : MonoBehaviour {
  [SerializeField] private SmallPlayer smallPlayer1;
  //[SerializeField] private SmallPlayer smallPlayer2;
  //[SerializeField] private SmallPlayer smallPlayer3;

  private void Update() {
    float xMove = XCI.GetAxis(XboxAxis.LeftStickX);
    float zMove = XCI.GetAxis(XboxAxis.LeftStickY);
    smallPlayer1.Move(xMove, zMove);

    float Xrotation = XCI.GetAxis(XboxAxis.RightStickX);
    float Zrotation = XCI.GetAxis(XboxAxis.RightStickY);
    if (!(Xrotation == 0 && Zrotation == 0)) {
      smallPlayer1.SetRotation(Xrotation, Zrotation);
    }
  }
}
