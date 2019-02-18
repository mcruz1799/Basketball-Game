using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

public class PlayerSelectionGUI : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Text countdown;

  [SerializeField] private Image controller1PlayerImage;
  [SerializeField] private Image controller2PlayerImage;
  [SerializeField] private Image controller3PlayerImage;
  [SerializeField] private Image controller4PlayerImage;

  [SerializeField] private Text controller1Prompt;
  [SerializeField] private Text controller2Prompt;
  [SerializeField] private Text controller3Prompt;
  [SerializeField] private Text controller4Prompt;
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
      Color textColor = isConfirmed ? Color.green : Color.red;
      string text = isConfirmed ? "CONFIRMED" : "Press Start";
      switch (info.Key) {
        case XboxController.First:
          controller1PlayerImage.sprite = sprite;
          controller1Prompt.color = textColor;
          controller1Prompt.text = text;
          break;
        case XboxController.Second:
          controller2PlayerImage.sprite = sprite;
          controller2Prompt.color = textColor;
          controller2Prompt.text = text;
          break;
        case XboxController.Third:
          controller3PlayerImage.sprite = sprite;
          controller3Prompt.color = textColor;
          controller3Prompt.text = text;
          break;
        case XboxController.Fourth:
          controller4PlayerImage.sprite = sprite;
          controller4Prompt.color = textColor;
          controller4Prompt.text = text;
          break;
        default:
          Debug.LogError("Illegal XboxController key detected in InputManager.ControllerMap");
          break;
      }
    }
  }
}
