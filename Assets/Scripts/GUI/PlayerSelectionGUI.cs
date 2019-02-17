using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

public class PlayerSelectionGUI : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private GameObject screenParent;
  [SerializeField] private Text countdown;

  [SerializeField] private Image controller1;
  [SerializeField] private Image controller2;
  [SerializeField] private Image controller3;
  [SerializeField] private Image controller4;
#pragma warning restore 0649

  private void Awake() {
    countdown.text = "";
  }

  private void Update() {
    UpdateGUI();
    
  }

  private void UpdateGUI() {
    countdown.text = InputManager.S.Countdown.ToString();

    foreach (KeyValuePair<XboxController, InputManager.PlayerSelectionInfo> info in InputManager.S.ControllerMap) {
      Sprite sprite = null;
      //Sprite sprite = info.Value.player.sprite
      switch (info.Key) {
        case XboxController.First:
          controller1.sprite = sprite;
          break;
        case XboxController.Second:
          controller1.sprite = sprite;
          break;
        case XboxController.Third:
          controller1.sprite = sprite;
          break;
        case XboxController.Fourth:
          controller1.sprite = sprite;
          break;
        default:
          Debug.LogError("Illegal XboxController key detected in InputManager.ControllerMap");
          break;
      }
    }
  }
}
