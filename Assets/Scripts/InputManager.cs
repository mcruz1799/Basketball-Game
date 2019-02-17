using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {
  public static InputManager S { get; private set; }

#pragma warning disable 0649
#pragma warning restore 0649

  public int Countdown { get; private set; }

  public struct PlayerSelectionInfo {
    public readonly IPlayer player;
    public readonly bool isConfirmed;

    public PlayerSelectionInfo(IPlayer player, bool isConfirmed) {
      this.player = player;
      this.isConfirmed = isConfirmed;
    }
  }
  private Dictionary<XboxController, PlayerSelectionInfo> controllerMap;
  public IReadOnlyDictionary<XboxController, PlayerSelectionInfo> ControllerMap => controllerMap;

  private void Awake() {
    if (S == null) {
      S = this;
      DontDestroyOnLoad(this);
    } else {
      Debug.LogWarning("Duplicate InputManager detected and destroyed.");
      Destroy(gameObject);
    }
    controllerMap = new Dictionary<XboxController, PlayerSelectionInfo>();
  }

  private IEnumerator InputReadingRoutine() {
    while (true) {
      if (!ControllerMapComplete()) {
        PlayerSelectionInputChecks(XboxController.First);
        PlayerSelectionInputChecks(XboxController.Second);
        PlayerSelectionInputChecks(XboxController.Third);
        PlayerSelectionInputChecks(XboxController.Fourth);
      } else {
        MainGameInputChecks(XboxController.First);
        MainGameInputChecks(XboxController.Second);
        MainGameInputChecks(XboxController.Third);
        MainGameInputChecks(XboxController.Fourth);
      }
      yield return null;
    }
  }

  //controllerMap is complete when 4 players are registered
  private bool ControllerMapComplete() {

    //There must be 4 players
    if (controllerMap.Count != 4) {
      return false;
    }

    //All 4 players must have confirmed their player selection
    foreach (PlayerSelectionInfo info in controllerMap.Values) {
      if (!info.isConfirmed) {
        return false;
      }
    }

    return true;
  }

  //Waits until all 4 players have selected who to play
  private IEnumerator PlayerSelectionRoutine() {
    while (true) {
      yield return new WaitUntil(ControllerMapComplete);

      //Wait 5 seconds after the game is ready
      for (float timer = 5f; timer > 0f; timer -= 0.1f) {
        yield return new WaitForSeconds(0.1f);

        //If someone cancels, then break.  The code will return to the start of the while-loop.
        if (!ControllerMapComplete()) {
          Countdown = 5;
          break;
        }

        Countdown = Mathf.CeilToInt(timer);
      }

      //If controllerMap is still completed (i.e. nobody has canceled), start the game
      if (ControllerMapComplete()) {
        Countdown = 0;
        yield break;
      }
    }
  }

  private void PlayerSelectionInputChecks(XboxController controller) {

    //Select a player with A/B/X/Y (does not confirm the selection)
    if (XCI.GetButtonDown(XboxButton.A)) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.SmallPlayer1, false);
    }
    if (XCI.GetButtonDown(XboxButton.B)) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.TallPlayer1, false);
    }
    if (XCI.GetButtonDown(XboxButton.X)) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.SmallPlayer2, false);
    }
    if (XCI.GetButtonDown(XboxButton.Y)) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.TallPlayer2, false);
    }

    //If you've selected a player, press Start to confirm it
    if (XCI.GetButtonDown(XboxButton.Start) && controllerMap.ContainsKey(controller)) {

      IPlayer selectedPlayer = controllerMap[controller].player;

      //Check if someone else is already using the player
      bool selectedPlayerIsAvailable = true;
      foreach (PlayerSelectionInfo info in controllerMap.Values) {
        if (info.isConfirmed && info.player == selectedPlayer) {
          selectedPlayerIsAvailable = false;
        }
      }

      //If the player is available, lock in your selection.
      if (selectedPlayerIsAvailable) {
        controllerMap[controller] = new PlayerSelectionInfo(selectedPlayer, true);
      }
    }
  }

  private void MainGameInputChecks(XboxController controller) {
    IPlayer player = controllerMap[controller].player;

    //Check left joystick
    float xMove = XCI.GetAxis(XboxAxis.LeftStickX, controller);
    float zMove = XCI.GetAxis(XboxAxis.LeftStickY, controller);
    player.Move(zMove, -xMove);
    if (!(xMove == 0 && zMove == 0)) {
      player.SetRotation(zMove, -xMove);
    }

    //Check button presses
    if (XCI.GetButtonDown(XboxButton.A, controller)) {
      player.AButtonDown(controller);
    }
    if (XCI.GetButtonDown(XboxButton.B, controller)) {
      player.BButtonDown(controller);
    }
    if (XCI.GetAxis(XboxAxis.RightTrigger, controller) >= .3f) {
      player.RTButtonDown(controller);
    }
    if (XCI.GetAxis(XboxAxis.RightTrigger, controller) < .3f) {
      player.RTButtonUp(controller);
    }
  }
}

//public class InputManager : MonoBehaviour {
//  private IPlayer[] players = new IPlayer[4];
//  private Dictionary<XboxController, int> controllerToPlayer;
//  private TeamPickerManager tpm;
//  private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };
//  private void Start() {
//    players[0] = GameManager.S.SmallPlayer1;
//    players[1] = GameManager.S.TallPlayer1;
//    players[2] = GameManager.S.SmallPlayer2;
//    players[3] = GameManager.S.TallPlayer2;

//    tpm = GameObject.Find("TeamPickerManager").GetComponent<TeamPickerManager>();
//    controllerToPlayer = tpm.controllerToPlayer;


//  }
//  private void Update() {
//    //For four controllers connected to the machine.

//    foreach (KeyValuePair<XboxController,int> kvp in controllerToPlayer)
//    {
//        CheckInputs(kvp.Key, players[kvp.Value]);
//    }
//  }
//    // for (int i = 0; i < 4; i++) {
//    //   try {
//    //     CheckInputs(controllers[i], players[i]);

//    //   } catch (System.Exception e) {
//    //     //TODO: Add UI message to prompt connecting controller.
//    //     //Debug.Log("Error receiving input.");
//    //     continue;
//    //   }
//    // }

//  //Checks for inputs from a specific controller, and applies movement to the s
//  private void CheckInputs(XboxController controller, IPlayer player) {
//    //Vector3 initialPos = new Vector3(player.X, 0, player.Z);
//    if (!GameManager.S.tipoff && !GameManager.S.end) {


//      //Check Movement Inputs
//      float xMove = XCI.GetAxis(XboxAxis.LeftStickX, controller);
//      float zMove = XCI.GetAxis(XboxAxis.LeftStickY, controller);
//      player.Move(zMove, -xMove);
//      if (!(xMove == 0 && zMove == 0)) {
//        player.SetRotation(zMove, -xMove);
//      }
//    }
//    //Check Rotation Inputs
//    /* float Xrotation = XCI.GetAxis(XboxAxis.RightStickX, controller);
//     float Zrotation = XCI.GetAxis(XboxAxis.RightStickY, controller);
//     if (!(Xrotation == 0 && Zrotation == 0)) {
//       player.SetRotation(Xrotation, Zrotation);
//     } */

//    //Check Button Presses
//    if (XCI.GetButtonDown(XboxButton.A, controller)) {
//      player.AButtonDown(controller);
//    }
//    if (XCI.GetButtonDown(XboxButton.B, controller)) {
//      player.BButtonDown(controller);
//    }
//    if (XCI.GetAxis(XboxAxis.RightTrigger, controller) >= .3f) {
//      player.RTButtonDown(controller);
//    }
//    if (XCI.GetAxis(XboxAxis.RightTrigger, controller) < .3f) {
//      player.RTButtonUp(controller);
//    }
//  }
//}