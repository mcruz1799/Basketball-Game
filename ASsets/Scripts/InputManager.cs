using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class InputManager : MonoBehaviour {
    //[SerializeField] private SmallPlayer smallPlayer1;
    //[SerializeField] private SmallPlayer smallPlayer2;
    //[SerializeField] private SmallPlayer smallPlayer3;
    //[SerializeField] private SmallPlayer smallPlayer4;
  public SmallPlayer[] players = new SmallPlayer[4];
  private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };

  private void Update() {

    for (int i = 0; i < 2; i++) //For four controllers connected to the machine.
    {
       checkInputs(controllers[i], players[i]);
    }
  }
  
  //Checks for inputs from a specific controller, and applies movement to the s
  private void checkInputs(XboxController controller, IXzController player)
  {

        float xMove = XCI.GetAxis(XboxAxis.LeftStickX,controller);
        float zMove = XCI.GetAxis(XboxAxis.LeftStickY,controller);
        player.Move(xMove, zMove);

        float Xrotation = XCI.GetAxis(XboxAxis.RightStickX,controller);
        float Zrotation = XCI.GetAxis(XboxAxis.RightStickY,controller);
        if (!(Xrotation == 0 && Zrotation == 0))
        {
            player.SetRotation(Xrotation, Zrotation);
        }
    }
}
