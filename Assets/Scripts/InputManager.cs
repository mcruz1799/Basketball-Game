using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class InputManager : MonoBehaviour
{
    private IPlayer[] players = new IPlayer[4];
    private ControllerManager cm;
    private Dictionary<XboxController,int> controllerToPlayer;
    private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };
    private void Start()
    {
        players[0] = GameManager.S.SmallPlayer1;
        players[1] = GameManager.S.TallPlayer1;
        players[2] = GameManager.S.SmallPlayer2;
        players[3] = GameManager.S.TallPlayer2;

        //controllerToPlayer = cm.controllerToPlayer;
    }
    private void Update() {
    //For four controllers connected to the machine.

    // foreach (KeyValuePair<XboxController,int> kvp in controllerToPlayer)
    // {
    //     checkInputs(kvp.Key, players[kvp.Value]);
    // }
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
    //Vector3 initialPos = new Vector3(player.X, 0, player.Z);
    if (!GameManager.S.tipoff && !GameManager.S.end){

    
      //Check Movement Inputs
      float xMove = XCI.GetAxis(XboxAxis.LeftStickX, controller);
      float zMove = XCI.GetAxis(XboxAxis.LeftStickY, controller);
      Vector3 newPos = player.Move(zMove, -xMove);
      if (!(xMove == 0 && zMove == 0))
      {
          player.SetRotation(zMove, -xMove);
      }
      if (Mathf.Abs(newPos.x) > Xboundary || Mathf.Abs(newPos.z) > Zboundary)
      {
        player.Move(-zMove, xMove);
      }
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
    //if (XCI.GetButtonDown(XboxButton.B, controller))
    //{
    //    player.PressB(controller);
    //}
    //Listen for Dash
    if (XCI.GetButton(XboxButton.B,controller))
    {
       Player p = (Player)player;
       if (p.currentDashState == Player.DashState.Default && p.dashRefillPenalty != true) //Start Dash Coroutine.
       {
         p.currentDashState = Player.DashState.Dash;
         p.StartCoroutine("DashActive");
       } else if (p.dashRefillPenalty == true) { p.currentDashState = Player.DashState.Default; } //Disable if under penalty
    } else if (XCI.GetButtonUp(XboxButton.B, controller)) //End Timer
    {
       Player p = (Player)player;
       p.currentDashState = Player.DashState.Default; //Disable upon release.
       p.StopCoroutine("DashActive");
       p.StartCoroutine("DashInactive");
    }
  }
}