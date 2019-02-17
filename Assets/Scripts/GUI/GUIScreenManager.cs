using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameState;

public class GUIScreenManager : MonoBehaviour {
  public static GUIScreenManager S { get; private set; }

#pragma warning disable 0649
  [SerializeField] private GameObject mainMenu;
  [SerializeField] private GameObject playerSelection;
  [SerializeField] private GameObject mainGame;
#pragma warning restore 0649

  private void Awake() {
    if (S == null) {
      S = this;
      DontDestroyOnLoad(this);
    } else {
      Debug.LogWarning("Duplicate GUIScreenManager detected and destroyed.");
      Destroy(gameObject);
    }
  }

  private void Update() {
    GameState state = GameManager.S.State;

    mainMenu.SetActive(state == MainMenu);
    playerSelection.SetActive(state == PlayerSelection);
    mainGame.SetActive(state == Tipoff || state == InPlay || state == Overtime || state == GameOver);
  }
}
