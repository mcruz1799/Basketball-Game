using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class InputManager : MonoBehaviour {
  //[SerializeField] private SmallPlayer smallPlayer1;
  //[SerializeField] private SmallPlayer smallPlayer2;
  //[SerializeField] private SmallPlayer smallPlayer3;
  //[SerializeField] private SmallPlayer smallPlayer4;
  public SmallPlayer[] players = new SmallPlayer[2];
  public TallPlayer[] players2 = new TallPlayer[2];
  private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };

  private void Update() {

    //For four controllers connected to the machine.
    for (int i = 0; i < 4; i++) {
      try
      {
        XboxController control = controllers[i];
        if (i % 2 == 0) checkInputs(controllers[i], players[i], "small");
        else checkInputs(controllers[i], players2[i], "tall");
      } catch (System.Exception e)
      {
        //TODO: Add UI message to prompt connecting controller.
        continue;
      } 
    }
  }

  //Checks for inputs from a specific controller, and applies movement to the s
  private void checkInputs(XboxController controller, IXzController player, string PlayerType) {
    
    //Check Movement Inputs
    float xMove = XCI.GetAxis(XboxAxis.LeftStickX, controller);
    float zMove = XCI.GetAxis(XboxAxis.LeftStickY, controller);
    player.Move(xMove, zMove);
    
    //Check Rotation Inputs
    float Xrotation = XCI.GetAxis(XboxAxis.RightStickX, controller);
    float Zrotation = XCI.GetAxis(XboxAxis.RightStickY, controller);
    if (!(Xrotation == 0 && Zrotation == 0)) {
      player.SetRotation(Xrotation, Zrotation);
    }

    //Check Button Presses
    if (XCI.GetButton(XboxButton.A,controller))
    {
      player.PressA(controller);
    }
  }
}