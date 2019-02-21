using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {
  public static InputManager S { get; private set; }

#pragma warning disable 0649
  [Range(1f, 10f)]
  [SerializeField] private float countdownLength;
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
    Countdown = Mathf.CeilToInt(countdownLength);
    if (S == null) {
      S = this;
      DontDestroyOnLoad(this);
    } else {
      Debug.LogWarning("Duplicate InputManager detected and destroyed.");
      Destroy(gameObject);
    }
    controllerMap = new Dictionary<XboxController, PlayerSelectionInfo>();

    //Default player selection
    Select(XboxController.First, SelectionAction.Small1);
    Select(XboxController.Second, SelectionAction.Tall1);
    Select(XboxController.Third, SelectionAction.Small2);
    Select(XboxController.Fourth, SelectionAction.Tall2);

    //For debugging
    Select(XboxController.First, SelectionAction.Confirm);
    Select(XboxController.Second, SelectionAction.Confirm);
    Select(XboxController.Third, SelectionAction.Confirm);
    Select(XboxController.Fourth, SelectionAction.Confirm);

    StartCoroutine(InputReadingRoutine());
    StartCoroutine(PlayerSelectionRoutine());
  }

  private void DebugInputChecks() {
    if (GameManager.S.State.PlayerMovementAllowed()) {
      IPlayer player = controllerMap[XboxController.Second].player;

      //Check left joystick
      float xMove = Input.GetKey(KeyCode.RightArrow) ? 1f : Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;
      float zMove = Input.GetKey(KeyCode.UpArrow) ? 1f : Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
      player.Move(zMove, -xMove);
      if (!(xMove == 0 && zMove == 0)) {
        player.SetRotation(zMove, -xMove);
      }

      //Check button presses
      if (Input.GetKeyDown(KeyCode.A)) {
        player.AButtonDown(XboxController.Second);
      }
      if (Input.GetKeyDown(KeyCode.B)) {
        player.BButtonDown(XboxController.Second);
      }
      //if (XCI.GetAxis(XboxAxis.RightTrigger, controller) >= .3f) {
      //  player.RTButtonDown(controller);
      //}
      //if (XCI.GetAxis(XboxAxis.RightTrigger, controller) < .3f) {
      //  player.RTButtonUp(controller);
      //}
    }
  }

  private IEnumerator InputReadingRoutine() {
    while (true) {
      //GameManager.S.State is checked inside of these functions, so no need to do it jere
      PlayerSelectionInputChecks(XboxController.First);
      PlayerSelectionInputChecks(XboxController.Second);
      PlayerSelectionInputChecks(XboxController.Third);
      PlayerSelectionInputChecks(XboxController.Fourth);
      MainGameInputChecks(XboxController.First);
      MainGameInputChecks(XboxController.Second);
      MainGameInputChecks(XboxController.Third);
      MainGameInputChecks(XboxController.Fourth);
      DebugInputChecks();

      yield return null;
    }
  }

  //controllerMap is complete when 4 players are registered
  public bool ControllerMapComplete() {

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
      yield return new WaitUntil(() => GameManager.S.State == GameState.PlayerSelection && ControllerMapComplete());

      //Wait 5 seconds after the game is ready
      for (float timer = countdownLength; timer > 0f; timer -= 0.1f) {
        yield return new WaitForSeconds(0.1f);

        //If someone cancels, then break.  The code will return to the start of the while-loop.
        if (!ControllerMapComplete()) {
          Countdown = Mathf.CeilToInt(countdownLength);
          break;
        }

        Countdown = Mathf.CeilToInt(timer);
      }

      //If controllerMap is still completed (i.e. nobody has canceled), start the game
      if (ControllerMapComplete()) {
        Countdown = 0;
        GameManager.S.EndPlayerSelection();
      }
    }
  }

  private enum SelectionAction { Small1, Tall1, Small2, Tall2, Confirm }

  private void PlayerSelectionInputChecks(XboxController controller) {
    if (GameManager.S.State == GameState.PlayerSelection) {

      //Select a player with A/B/X/Y (does not confirm the selection)
      if (XCI.GetButtonDown(XboxButton.A, controller)) {
        Select(controller, SelectionAction.Small1);
      }
      if (XCI.GetButtonDown(XboxButton.B, controller)) {
        Select(controller, SelectionAction.Tall1);
      }
      if (XCI.GetButtonDown(XboxButton.X, controller)) {
        Select(controller, SelectionAction.Small2);
      }
      if (XCI.GetButtonDown(XboxButton.Y, controller)) {
        Select(controller, SelectionAction.Tall2);
      }

      //If you've selected a player, press Start to confirm it
      if (XCI.GetButtonDown(XboxButton.Start, controller)) {
        Select(controller, SelectionAction.Confirm);
      }
    }
  }

  private void Select(XboxController controller, SelectionAction action) {
    //Select a player with A/B/X/Y (does not confirm the selection)
    if (action == SelectionAction.Small1) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.SmallPlayer1, false);
    }
    if (action == SelectionAction.Tall1) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.TallPlayer1, false);
    }
    if (action == SelectionAction.Small2) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.SmallPlayer2, false);
    }
    if (action == SelectionAction.Tall2) {
      controllerMap[controller] = new PlayerSelectionInfo(GameManager.S.TallPlayer2, false);
    }

    //If you've selected a player, press Start to confirm it
    if (action == SelectionAction.Confirm && controllerMap.ContainsKey(controller)) {
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
    if (GameManager.S.State.PlayerMovementAllowed()) {
      IPlayer player = controllerMap[controller].player;

      float threshold = 0.5f;
      //Check left joystick
      float xMove = XCI.GetAxis(XboxAxis.LeftStickX, controller);
      xMove = xMove < -threshold ? -1f : xMove > threshold ? 1f : 0f;

      float zMove = XCI.GetAxis(XboxAxis.LeftStickY, controller);
      zMove = zMove < -threshold ? -1f : zMove > threshold ? 1f : 0f;

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
}