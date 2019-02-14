using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class InputManager : MonoBehaviour {
  private IPlayer[] players = new IPlayer[4];
  private Dictionary<XboxController, int> controllerToPlayer;
  private TeamPickerManager tpm;
  private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };
  private void Start() {
    players[0] = GameManager.S.SmallPlayer1;
    players[1] = GameManager.S.TallPlayer1;
    players[2] = GameManager.S.SmallPlayer2;
    players[3] = GameManager.S.TallPlayer2;

    //controllerToPlayer = tpm.controllerToPlayer;


  }
  private void Update() {
    //For four controllers connected to the machine.

    // foreach (KeyValuePair<XboxController,int> kvp in controllerToPlayer)
    // {
    //     checkInputs(kvp.Key, players[kvp.Value]);
    // }
    for (int i = 0; i < 4; i++) {
      try {
        CheckInputs(controllers[i], players[i]);

      } catch (System.Exception e) {
        //TODO: Add UI message to prompt connecting controller.
        //Debug.Log("Error receiving input.");
        continue;
      }
    }
  }

  //Checks for inputs from a specific controller, and applies movement to the s
  private void CheckInputs(XboxController controller, IPlayer player) {
    //Vector3 initialPos = new Vector3(player.X, 0, player.Z);
    if (!GameManager.S.tipoff && !GameManager.S.end) {


      //Check Movement Inputs
      float xMove = XCI.GetAxis(XboxAxis.LeftStickX, controller);
      float zMove = XCI.GetAxis(XboxAxis.LeftStickY, controller);
      player.Move(zMove, -xMove);
      if (!(xMove == 0 && zMove == 0)) {
        player.SetRotation(zMove, -xMove);
      }
    }
    //Check Rotation Inputs
    /* float Xrotation = XCI.GetAxis(XboxAxis.RightStickX, controller);
     float Zrotation = XCI.GetAxis(XboxAxis.RightStickY, controller);
     if (!(Xrotation == 0 && Zrotation == 0)) {
       player.SetRotation(Xrotation, Zrotation);
     } */

    //Check Button Presses
    if (XCI.GetButtonDown(XboxButton.A, controller)) {
      player.AButtonDown(controller);
    }
    if (XCI.GetButtonDown(XboxButton.B, controller)) {
      player.BButtonDown(controller);
    }
    if (XCI.GetButtonUp(XboxButton.B, controller)) {
      player.BButtonUp(controller);
    }
  }
}