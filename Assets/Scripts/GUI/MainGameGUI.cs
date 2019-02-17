using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager.State;

public class MainGameGUI : MonoBehaviour {
  public static MainGameGUI S;

#pragma warning disable 0649
  [SerializeField] private GameObject winningScreen;
  [SerializeField] private GameObject tipoffScreen;
  [SerializeField] private GameObject overtimeScreen;

  //GUI objects
  [SerializeField] private Text winningText;
  [SerializeField] private Text gameTimeText;
  [SerializeField] private Text team1ScoreText;
  [SerializeField] private Text team2ScoreText;
  [SerializeField] private RawImage possessionIndicator1;
  [SerializeField] private RawImage possessionIndicator2;
#pragma warning disable 0649

  private void Awake() {
    if (S == null) {
      S = this;
      DontDestroyOnLoad(this);
    } else {
      Debug.LogWarning("Duplicate MainGameGUI detected and destroyed.");
      Destroy(gameObject);
    }
  }

  public void UpdateAll() {
    UpdateScreens();
    UpdateScores();
    UpdateTime();
  }

  public void ShowOvertimeScreen(float displayTime) {
    if (GameManager.S.state != Overtime) {
      Debug.LogError("Showing overtime screen even though it isn't overtime!");
    }
    StartCoroutine(ShowOvertimeScreenRoutine(displayTime));
  }
  private IEnumerator ShowOvertimeScreenRoutine(float displayTime) {
    overtimeScreen.SetActive(true);
    yield return new WaitForSeconds(displayTime);
    overtimeScreen.SetActive(false);
  }

  public void UpdateScreens() {
    winningScreen.SetActive(GameManager.S.state == GameOver);
    tipoffScreen.SetActive(GameManager.S.state == Tipoff);
    

    switch (GameManager.S.state) {
      case Initializing:
        break;

      case Tipoff:
        break;

      case Overtime:
        break;

      case GameOver:
        if (GameManager.S.ScoreTeam2 > GameManager.S.ScoreTeam1) {
          S.winningText.text = "Congratulations to \nTeam Blue!";
        } else if (GameManager.S.ScoreTeam1 > GameManager.S.ScoreTeam2) {
          S.winningText.text = "Congratulations to \nTeam Red!";
        }
        break;

      default:
        Debug.LogError("Illegal GameManager.State enum value detected");
        break;
    }

    
  }

  public void UpdateScores() {
    team1ScoreText.text = GameManager.S.ScoreTeam1.ToString();
    team2ScoreText.text = GameManager.S.ScoreTeam2.ToString();
  }

  public void UpdateTime() {
    float time = GameManager.S.CurrentTime;
    float minutes = Mathf.Floor(time / 60);
    float seconds = time % 60;
    string min_str = minutes.ToString();
    string sec_str = seconds.ToString();
    if (minutes < 10) min_str = "0" + min_str;
    if (seconds < 10) sec_str = "0" + sec_str;
    gameTimeText.text = min_str + ":" + sec_str;
  }

  public void UpdatePossessionIndicator(ScoreComponent.PlayerType? team) {
    if (team == null) {
      possessionIndicator1.gameObject.SetActive(false);
      possessionIndicator2.gameObject.SetActive(false);
    } else {
      switch (team.Value) {
        case ScoreComponent.PlayerType.team1:
          possessionIndicator1.gameObject.SetActive(true);
          possessionIndicator2.gameObject.SetActive(false);
          return;

        case ScoreComponent.PlayerType.team2:
          possessionIndicator1.gameObject.SetActive(false);
          possessionIndicator2.gameObject.SetActive(true);
          return;

        default:
          Debug.LogWarning("Illegal enum value detected");
          break;
      }
    }
  }
}
