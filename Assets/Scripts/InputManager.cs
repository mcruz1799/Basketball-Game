using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class InputManager : MonoBehaviour
{
    private IPlayer[] players = new IPlayer[4];
    private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };


    private void Start()
    {
        players[0] = GameManager.S.SmallPlayer1;
        players[1] = GameManager.S.TallPlayer1;
        players[2] = GameManager.S.SmallPlayer2;
        players[3] = GameManager.S.TallPlayer2;
    }
    private void Update() {

    //For four controllers connected to the machine.
    for (int i = 0; i < 4; i++) {
      try
      {
                checkInputs(controllers[i], players[i]);
      } catch (System.Exception e)
      {
        //TODO: Add UI message to prompt connecting controller.
        //Debug.Log("Error receiving input.");
        continue;
      } 
    }
  }

  //Checks for inputs from a specific controller, and applies movement to the s
  private void checkInputs(XboxController controller, IPlayer player) {
    
    //Check Movement Inputs
    float xMove = XCI.GetAxis(XboxAxis.LeftStickX, controller);
    float zMove = XCI.GetAxis(XboxAxis.LeftStickY, controller);
    player.Move(zMove, -xMove);
    if (!(xMove == 0 && zMove == 0))
    {
        player.SetRotation(zMove, -xMove);
    }
    
    //Check Rotation Inputs
   /* float Xrotation = XCI.GetAxis(XboxAxis.RightStickX, controller);
    float Zrotation = XCI.GetAxis(XboxAxis.RightStickY, controller);
    if (!(Xrotation == 0 && Zrotation == 0)) {
      player.SetRotation(Xrotation, Zrotation);
    } */

    //Check Button Presses
    if (XCI.GetButtonDown(XboxButton.A,controller))
    {
      player.PressA(controller);
    }
    if (XCI.GetButtonDown(XboxButton.B, controller))
    {
        player.PressB(controller);
    }
    }
}