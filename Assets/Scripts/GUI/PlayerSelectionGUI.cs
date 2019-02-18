using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

public class PlayerSelectionGUI : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Text countdown;

  [SerializeField] private Image controller1;
  [SerializeField] private Image controller2;
  [SerializeField] private Image controller3;
  [SerializeField] private Image controller4;

  [SerializeField] private Text controller1Confirmed;
  [SerializeField] private Text controller2Confirmed;
  [SerializeField] private Text controller3Confirmed;
  [SerializeField] private Text controller4Confirmed;
#pragma warning restore 0649

  private void Awake() {
    countdown.text = "";
  }

  private void Update() {
    UpdateGUI();
  }

  private void UpdateGUI() {
    if (InputManager.S.ControllerMapComplete()) {
      countdown.text = InputManager.S.Countdown.ToString();
    } else {
      countdown.text = "";
    }

    foreach (KeyValuePair<XboxController, InputManager.PlayerSelectionInfo> info in InputManager.S.ControllerMap) {
      Sprite sprite = info.Value.player.Icon;
      bool isConfirmed = info.Value.isConfirmed;
      switch (info.Key) {
        case XboxController.First:
          controller1.sprite = sprite;
          controller1Confirmed.enabled = isConfirmed;
          break;
        case XboxController.Second:
          controller2.sprite = sprite;
          controller2Confirmed.enabled = isConfirmed;
          break;
        case XboxController.Third:
          controller3.sprite = sprite;
          controller3Confirmed.enabled = isConfirmed;
          break;
        case XboxController.Fourth:
          controller4.sprite = sprite;
          controller4Confirmed.enabled = isConfirmed;
          break;
        default:
          Debug.LogError("Illegal XboxController key detected in InputManager.ControllerMap");
          break;
      }
    }
  }
}
